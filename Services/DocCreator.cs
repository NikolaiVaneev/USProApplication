﻿using OfficeOpenXml;
using Spire.Doc;
using Spire.Doc.Documents;
using System.IO;
using USProApplication.DataBase.Entities;
using USProApplication.Models;
using USProApplication.Models.Repositories;
using USProApplication.Utils;

namespace USProApplication.Services
{
    public class DocCreator(ICounterpartyRepository counterpartyRepository) : IDocCreator
    {
        public async Task CreateActAsync(OrderDTO order, bool stamp)
        {
            string templatePath = Path.Combine("Templates", "Act.docx");
            string outputPath;

            Document doc = new();
            try
            {
                doc.LoadFromFile(templatePath);
            }
            catch (Exception)
            {
                throw new Exception("Невозможно открыть шаблон документа. Вероятно, он отсутствует в папке Templates.");
            }

            if (!stamp) RemoveImages(doc);


            CounterpartyDTO? executor;
            CounterpartyDTO? client;

            if (order.ParentId == null)
            {
                outputPath = Path.Combine(Path.GetTempPath(), $"Акт {order.Number!.Replace('/', '_')}-{order.Name}.docx");
                client = await counterpartyRepository.GetByIdAsync((Guid)order.CustomerId!);
                executor = await counterpartyRepository.GetByIdAsync((Guid)order.ExecutorId!);

                doc.Replace("{Address}", order.Address, true, true);
                doc.Replace("{NDS}", GetNDSDescription(order), true, true);
            }
            else
            {
                outputPath = Path.Combine(Path.GetTempPath(), $"Акт ДС {order.Number!.Replace('/', '_')}-{order.Name}.docx");
                client = await counterpartyRepository.GetByIdAsync((Guid)order.ParentOrder!.CustomerId!);
                executor = await counterpartyRepository.GetByIdAsync((Guid)order.ParentOrder!.ExecutorId!);

                doc.Replace("{Address}", order.ParentOrder.Address, true, true);
                doc.Replace("{NDS}", GetNDSDescription(order.ParentOrder), true, true);
            }

            doc.Replace("{ContractNumber}", order.Number, true, true);
            doc.Replace("{ContractDate}", DateConverter.ConvertDateToString(order.StartDate), true, true);
            doc.Replace("{Date}", DateConverter.ConvertDateToString(order.СompletionDate), true, true);
            doc.Replace("{Price}", string.Format("{0:N2}", order.Price), true, true);
            doc.Replace("{FullPrice}", DecimalConverter.ConvertDecimalToString(order.Price), true, true);

            var morpherService = new MorpherService();

            doc.Replace("{ClientOrg}", client!.Name, true, true);
            doc.Replace("{ClientFullName}", await morpherService.GetDeclensionAsync(client.Director, MorpherService.RussianCase.Accusative), true, true);
            doc.Replace("{ClientPosition}", GetDirectorPosition(client.DirectorPosition, false), true, true);
            doc.Replace("{ClientShortName}", await morpherService.GetShortNameAsync(client.Director, MorpherService.RussianCase.Nominative), true, true);

            doc.Replace("{ExecutorOrg}", executor!.Name, true, true);
            doc.Replace("{ExecutorFullName}", await morpherService.GetDeclensionAsync(executor.Director, MorpherService.RussianCase.Accusative), true, true);
            doc.Replace("{ExecutorPosition}", GetDirectorPosition(executor.DirectorPosition, false), true, true);
            doc.Replace("{ExecutorShortName}", await morpherService.GetShortNameAsync(executor.Director, MorpherService.RussianCase.Nominative), true, true);

            try
            {
                doc.SaveToFile(outputPath);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(outputPath) { UseShellExecute = true });
            }
            catch (Exception)
            {
                throw new Exception("Невозможно сохранить акт. Вероятно, он уже открыт. Закройте документ и попробуйте снова");
            }
        }

        public async Task CreateContractAsync(OrderDTO order, bool stamp)
        {
            string templatePath = Path.Combine("Templates", "Contract.docx");
            string outputPath = Path.Combine(Path.GetTempPath(), $"Договор {order.Number!.Replace('/', '_')}-{order.Name}.docx");

            Document doc = new();
            try
            {
                doc.LoadFromFile(templatePath);
            }
            catch (Exception)
            {
                throw new Exception("Невозможно открыть шаблон документа. Вероятно, он отсутствует в папке Templates.");
            }

            try
            {
                doc.SaveToFile(outputPath);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(outputPath) { UseShellExecute = true });
            }
            catch (Exception)
            {
                throw new Exception("Невозможно сохранить договор. Вероятно, он уже открыт. Закройте документ и попробуйте снова");
            }
        }

        public async Task CreateContractInvoiceAsync(OrderDTO order, bool stamp)
        {
            string templatePath = Path.Combine("Templates", "ContractBill.docx");
            string outputPath = Path.Combine(Path.GetTempPath(), $"Договор-счет {order.Number!.Replace('/', '_')}-{order.Name}.docx");

            Document doc = new();
            try
            {
                doc.LoadFromFile(templatePath);
            }
            catch (Exception)
            {
                throw new Exception("Невозможно открыть шаблон документа. Вероятно, он отсутствует в папке Templates.");
            }

            if (!stamp) RemoveImages(doc);

            doc.Replace("{Number}", order.Number, true, true);
            doc.Replace("{Address}", order.Address, true, true);
            doc.Replace("{Service}", order.AdditionalService, true, true);
            doc.Replace("{Square}", order.Square.ToString(), true, true);
            doc.Replace("{Deadline}", GetDeadline(order.Square), true, true);
            doc.Replace("{Date}", $"{DateConverter.ConvertDateToString(DateTime.Now)} г.", true, true);
            doc.Replace("{Price}", string.Format("{0:N2}", order.Price), true, true);
            doc.Replace("{FullPrice}", DecimalConverter.ConvertDecimalToString(order.Price), true, true);

            if (order.UsingNDS && order.NDS > 0)
            {
                var tax = Math.Round((decimal)(order.Price! * order.NDS / (100 + order.NDS)), 2);
                doc.Replace("{NDSType}", $"В том числе НДС {order.NDS}%", true, true);
                doc.Replace("{NDS}", string.Format("{0:N2}", tax), true, true);
                doc.Replace("{NDSNotExist}", string.Empty, true, true);
                doc.Replace("{NDSExist}", $"В том числе НДС {order.NDS}% {string.Format("{0:N2}", tax)} ({DecimalConverter.ConvertDecimalToString(tax)}) рублей", true, true);
            }
            else
            {
                doc.Replace("{NDSNotExist}", "НДС не облагается (Уведомление о возможности применения УСН № 2490 от 03.12.2007 г.)", true, true);
                doc.Replace("{NDSExist}", string.Empty, true, true);
                doc.Replace("{NDSType}", "Без налога (НДС)", true, true);
                doc.Replace("{NDS}", "-", true, true);
            }

            var client = await counterpartyRepository.GetByIdAsync((Guid)order.CustomerId!);
            if (client != null)
            {
                doc.Replace("{ClientName}", client.Name, true, true);
                doc.Replace("{ClientAddress}", client.Address, true, true);
                doc.Replace("{ClientINN}", client.INN, true, true);
                doc.Replace("{ClientKPP}", client.KPP, true, true);
                doc.Replace("{Email}", order.Email, true, true);
            }

            var executor = await counterpartyRepository.GetByIdAsync((Guid)order.ExecutorId!);
            if (executor != null)
            {
                doc.Replace("{ExecutorName}", executor.Name, true, true);
                doc.Replace("{ExecutorAddress}", executor.Address, true, true);

                doc.Replace("{Bank}", executor.Bank, true, true);
                doc.Replace("{BIK}", executor.BIK, true, true);
                doc.Replace("{OGRN}", executor.OGRN, true, true);
                doc.Replace("{INN}", executor.INN, true, true);
                doc.Replace("{KPP}", executor.KPP, true, true);
                doc.Replace("{CorrAccount}", executor.CorrAccount, true, true);
                doc.Replace("{Account}", executor.PaymentAccount, true, true);

                doc.Replace("{Recipient}", executor.Name, true, true);
                doc.Replace("{Executor}", $"{executor.Name}, ИНН {executor.INN}, КПП {executor.KPP}, {executor.Address}", true, true);
            }


            try
            {
                doc.SaveToFile(outputPath);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(outputPath) { UseShellExecute = true });
            }
            catch (Exception)
            {
                throw new Exception("Невозможно сохранить договор-счет. Вероятно, он уже открыт. Закройте документ и попробуйте снова");
            }
        }

        public async Task CreatePaymentInvoiceAsync(OrderDTO order, PaymentInvioceTypes type, bool stamp)
        {
            string templatePath = Path.Combine("Templates", "Bill.docx");
            string outputPath = string.Empty;
            string number = string.Empty;
            decimal price = 0;
            string billType = string.Empty;

            Document doc = new();
            try
            {
                doc.LoadFromFile(templatePath);
            }
            catch (Exception)
            {
                throw new Exception("Невозможно открыть шаблон документа. Вероятно, он отсутствует в папке Templates.");
            }

            if (!stamp) RemoveImages(doc);

            switch (type)
            {
                case PaymentInvioceTypes.Prepayment:
                    outputPath = Path.Combine(Path.GetTempPath(), $"Счет (предоплата) {order.PrepaymentBillNumber} от {order.PrepaymentBillDate:dd.MM.yyyy} {order.Number!.Replace('/', '_')}-{order.Name}.docx");
                    number = $"{order.PrepaymentBillNumber} от {DateConverter.ConvertDateToString(order.PrepaymentBillDate)} г.";
                    price = (decimal)(order.Price! * order.PrepaymentPercent! / 100);
                    billType = $"Предоплата ({order.PrepaymentPercent}%)";
                    break;
                case PaymentInvioceTypes.Execution:
                    outputPath = Path.Combine(Path.GetTempPath(), $"Счет (выполнение) {order.ExecutionBillNumber} от {order.ExecutionBillDate:dd.MM.yyyy} {order.Number!.Replace('/', '_')}-{order.Name}.docx");
                    number = $"{order.ExecutionBillNumber} от {DateConverter.ConvertDateToString(order.ExecutionBillDate)} г.";
                    price = (decimal)(order.Price! * order.ExecutionPercent! / 100);
                    billType = $"Оплата ({order.ExecutionPercent}%)";
                    break;
                case PaymentInvioceTypes.Approval:
                    outputPath = Path.Combine(Path.GetTempPath(), $"Счет (согласование) {order.ApprovalBillNumber} от {order.ApprovalBillNumber:dd.MM.yyyy} {order.Number!.Replace('/', '_')}-{order.Name}.docx");
                    number = $"{order.ApprovalBillNumber} от {DateConverter.ConvertDateToString(order.ApprovalBillDate)} г.";
                    price = (decimal)(order.Price! * order.ApprovalPercent! / 100);
                    billType = $"Оплата ({order.ApprovalPercent}%)";
                    break;
            }

            doc.Replace("{Contract}", $"{order.Number} от {order.StartDate:dd.MM.yyyy} г.", true, true);
            doc.Replace("{Number}", number, true, true);

            doc.Replace("{Price}", string.Format("{0:N2}", price), true, true);
            doc.Replace("{FullPrice}", DecimalConverter.ConvertDecimalToString(price), true, true);
            doc.Replace("{PayType}", billType, true, true);

            CounterpartyDTO? executor;
            CounterpartyDTO? client;
            if (order.ParentId == null)
            {
                doc.Replace("{Object}", order.Name, true, true);
                doc.Replace("{AdditionalOrder}", string.Empty, true, true);

                if (order.UsingNDS && order.NDS > 0)
                {
                    var tax = Math.Round(price! * order.NDS / (100 + order.NDS), 2);
                    doc.Replace("{NDSType}", "В том числе НДС:", true, true);
                    doc.Replace("{NDS}", string.Format("{0:N2}", tax), true, true);
                }
                else
                {
                    doc.Replace("{NDSType}", "Без налога(НДС)", true, true);
                    doc.Replace("{NDS}", "-", true, true);
                }

                client = await counterpartyRepository.GetByIdAsync((Guid)order.CustomerId!);
                executor = await counterpartyRepository.GetByIdAsync((Guid)order.ExecutorId!);
            }
            else
            {
                doc.Replace("{Object}", order.ParentOrder!.Name, true, true);
                doc.Replace("{AdditionalOrder}", $"доп. соглашению № {order.Number} от {order.StartDate:dd.MM.yyyy} по ", true, true);

                if (order.ParentOrder!.UsingNDS && order.ParentOrder!.NDS > 0)
                {
                    var tax = Math.Round(price! * order.ParentOrder!.NDS / (100 + order.ParentOrder!.NDS), 2);
                    doc.Replace("{NDSType}", "В том числе НДС:", true, true);
                    doc.Replace("{NDS}", string.Format("{0:N2}", tax), true, true);
                }
                else
                {
                    doc.Replace("{NDSType}", "Без налога(НДС)", true, true);
                    doc.Replace("{NDS}", "-", true, true);
                }

                client = await counterpartyRepository.GetByIdAsync((Guid)order.ParentOrder!.CustomerId!);
                executor = await counterpartyRepository.GetByIdAsync((Guid)order.ParentOrder!.ExecutorId!);
            }

            var morpherService = new MorpherService();

            doc.Replace("{Client}", $"{client!.Name}, ИНН {client.INN}, КПП {client.KPP}, {client.Address}", true, true);
            doc.Replace("{Bank}", executor!.Bank, true, true);
            doc.Replace("{BIK}", executor.BIK, true, true);
            doc.Replace("{INN}", executor.INN, true, true);
            doc.Replace("{KPP}", executor.KPP, true, true);
            doc.Replace("{CorrAccount}", executor.CorrAccount, true, true);
            doc.Replace("{Account}", executor.PaymentAccount, true, true);

            doc.Replace("{Recipient}", executor.Name, true, true);
            doc.Replace("{Executor}", $"{executor.Name}, ИНН {executor.INN}, КПП {executor.KPP}, {executor.Address}", true, true);
            doc.Replace("{ExecutorShortName}", await morpherService.GetShortNameAsync(executor.Director, MorpherService.RussianCase.Nominative), true, true);



            try
            {
                doc.SaveToFile(outputPath);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(outputPath) { UseShellExecute = true });
            }
            catch (Exception)
            {
                throw new Exception("Невозможно сохранить счёт. Вероятно, он уже открыт. Закройте документ и попробуйте снова");
            }
        }

        public async Task CreateUPDAsync(OrderDTO order, bool stamp)
        {
            string templatePath = Path.Combine("Templates", "UPD.xlsx");
            string outputPath;
            FileInfo fileInfo = new(templatePath);

            using ExcelPackage package = new(fileInfo);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

            // Заполнение данных
            worksheet.Cells["Y1"].Value = $"{DateConverter.ConvertDateToString(DateTime.Now)} г.";
            worksheet.Cells["BF15"].Value = order.Price;
            worksheet.Cells["O31"].Value = $"{DateConverter.ConvertDateToString(order.СompletionDate)} года";



            CounterpartyDTO client = new();
            CounterpartyDTO executor = new();

            if (order.ParentId == null)
            {
                outputPath = Path.Combine(Path.GetTempPath(), $"УПД {order.Number!.Replace('/', '_')}-{order.Name}.xlsx");

                if (order.UsingNDS && order.NDS > 0)
                {
                    worksheet.Cells["AZ15"].Value = $"{order.NDS}%";

                    var tax = Math.Round((decimal)order.Price! * order.NDS / (100 + order.NDS), 2);
                    worksheet.Cells["BB15"].Value = tax;
                }

                worksheet.Cells["J15"].Value = $"Оказание услуг по разработке проектной документации согласно договору №USР-{order.Number} от {order.StartDate:dd.MM.yyyy} г. {order.Name}";
                worksheet.Cells["T22"].Value = $"USР-{order.Number} от {order.StartDate:dd.MM.yyyy} г.";

                client = await counterpartyRepository.GetByIdAsync(order.CustomerId!.Value);
                executor = await counterpartyRepository.GetByIdAsync(order.ExecutorId!.Value);

            }
            else
            {
                outputPath = Path.Combine(Path.GetTempPath(), $"УПД ДС {order.Number!.Replace('/', '_')}-{order.Name}.xlsx");

                if (order.ParentOrder!.UsingNDS && order.ParentOrder!.NDS > 0)
                {
                    worksheet.Cells["AZ15"].Value = $"{order.ParentOrder!.NDS}%";

                    var tax = Math.Round((decimal)order.Price! * order.ParentOrder!.NDS / (100 + order.ParentOrder!.NDS), 2);
                    worksheet.Cells["BB15"].Value = tax;
                }

                worksheet.Cells["J15"].Value = $"Оказание услуг по разработке проектной документации согласно доп. соглашению №{order.Number} от {order.StartDate:dd.MM.yyyy} г. по дог. №USР-{order.ParentOrder!.Number} от {order.ParentOrder!.StartDate:dd.MM.yyyy} г. {order.Name}";
                worksheet.Cells["T22"].Value = $"ДС {order.Number} от {order.StartDate:dd.MM.yyyy} к USР-{order.ParentOrder!.Number} от {order.ParentOrder!.StartDate:dd.MM.yyyy}";

                client = await counterpartyRepository.GetByIdAsync(order.ParentOrder!.CustomerId!.Value);
                executor = await counterpartyRepository.GetByIdAsync(order.ParentOrder!.ExecutorId!.Value);
            }

            worksheet.Cells["BE4"].Value = client!.Name;
            worksheet.Cells["BE5"].Value = client.Address;
            worksheet.Cells["BE6"].Value = $"{client.INN}/{client.KPP}";
            worksheet.Cells["AS40"].Value = $"{client.Name}, ИНН/КПП {client.INN}/{client.KPP}";

            worksheet.Cells["R4"].Value = executor!.Name;
            worksheet.Cells["R5"].Value = executor.Address;
            worksheet.Cells["R6"].Value = $"{executor.INN}/{executor.KPP}";
            worksheet.Cells["C40"].Value = $"{executor.Name}, ИНН/КПП {executor.INN}/{executor.KPP}";

            // Сохранение документа
            try
            {
                FileInfo outputFile = new(outputPath);
                package.SaveAs(outputFile);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(outputPath) { UseShellExecute = true });
            }
            catch (Exception)
            {
                throw new Exception("Невозможно сохранить УПД. Вероятно, он уже открыт. Закройте документ и попробуйте снова");
            }
        }

        private static string GetDeadline(int number)
        {
            int term = number;
            if (number == 0) return "нуля";

            string[] units = { "", "одного", "двух", "трех", "четырех", "пяти", "шести", "семи", "восьми", "девяти" };
            string[] teens = { "десяти", "одиннадцати", "двенадцати", "тринадцати", "четырнадцати", "пятнадцати", "шестнадцати", "семнадцати", "восемнадцати", "девятнадцати" };
            string[] tens = { "", "", "двадцати", "тридцати", "сорока", "пятидесяти", "шестидесяти", "семидесяти", "восьмидесяти", "девяноста" };
            string[] hundreds = { "", "ста", "двухсот", "трехсот", "четырехсот", "пятисот", "шестисот", "семисот", "восьмисот", "девятисот" };

            var parts = new List<string>();

            if (number >= 100)
            {
                int hundredPart = number / 100;
                parts.Add(hundreds[hundredPart]);
                number %= 100;
            }

            if (number >= 20)
            {
                int tensPart = number / 10;
                parts.Add(tens[tensPart]);
                number %= 10;
            }

            if (number >= 10)
            {
                parts.Add(teens[number - 10]);
                number = 0;
            }

            if (number > 0)
            {
                parts.Add(units[number]);
            }

            return $"{term} ({string.Join(" ", parts)})";
        }
        /// <summary>
        /// Получить описание НДС
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private static string GetNDSDescription(OrderDTO order)
        {
            string notNDS = "НДС не облагается (Уведомление о возможности применения УСН № 2490 от 03.12.2007 г.)";

            if (order.UsingNDS && order.NDS > 0)
            {
                var tax = Math.Round((decimal)order.Price! * order.NDS / (100 + order.NDS), 2);
                return $"в том числе НДС {order.NDS}% ({string.Format("{0:N2}", tax)} рублей)";
            }
            else
            {
                return notNDS;
            }
        }

        /// <summary>
        /// Удалить изображения из документа
        /// </summary>
        /// <param name="document"></param>
        private static void RemoveImages(Document document)
        {
            foreach (Section section in document.Sections)
            {
                foreach (Paragraph paragraph in section.Paragraphs)
                {
                    for (int i = paragraph.ChildObjects.Count - 1; i >= 0; i--)
                    {
                        DocumentObject obj = paragraph.ChildObjects[i];
                        if (obj.DocumentObjectType == DocumentObjectType.Picture)
                        {
                            paragraph.ChildObjects.RemoveAt(i);
                        }
                    }
                }

                // Удаление изображений из таблиц
                foreach (Table table in section.Tables)
                {
                    foreach (TableRow row in table.Rows)
                    {
                        foreach (TableCell cell in row.Cells)
                        {
                            for (int i = cell.Paragraphs.Count - 1; i >= 0; i--)
                            {
                                Paragraph paragraph = cell.Paragraphs[i];
                                for (int j = paragraph.ChildObjects.Count - 1; j >= 0; j--)
                                {
                                    DocumentObject obj = paragraph.ChildObjects[j];
                                    if (obj.DocumentObjectType == DocumentObjectType.Picture)
                                    {
                                        paragraph.ChildObjects.RemoveAt(j);
                                    }
                                }
                            }
                        }
                    }
                }
            }


        }

        /// <summary>
        /// Получить должность директора
        /// </summary>
        /// <param name="directorPosition">Должность</param>
        /// <param name="nominative">Именительный падеж</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static string GetDirectorPosition(DirectorPositions? directorPosition, bool nominative)
        {
            if (directorPosition == null)
            {
                return string.Empty;
            }

            return directorPosition switch
            {
                DirectorPositions.Director => nominative ? "Директор " : "Директора ",
                DirectorPositions.GeneralDirector => nominative ? "Генеральный директор " : "Генерального директора ",
                DirectorPositions.Manager => nominative ? "Управляющий " : "Управляющего ",
                DirectorPositions.Chief => nominative ? "Начальник " : "Начальника ",
                DirectorPositions.None => string.Empty,
                _ => throw new NotImplementedException()
            };
        }
    }
}
