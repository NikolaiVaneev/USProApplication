using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using OfficeOpenXml;
using Spire.Doc;
using Spire.Doc.Documents;
using System.IO;
using System.Text;
using USProApplication.DataBase.Entities;
using USProApplication.Models;
using USProApplication.Models.Repositories;
using USProApplication.Utils;

using Document = Spire.Doc.Document;
using Service = USProApplication.Models.Service;
using W = DocumentFormat.OpenXml.Wordprocessing;
using WP = DocumentFormat.OpenXml.Drawing.Wordprocessing;

namespace USProApplication.Services
{
    public class DocCreator(ICounterpartyRepository counterpartyRepository, IBaseRepository<Service> serviceRepository) : IDocCreator
    {
        private const string USPro_INN = "7703625087";
        private const string USProject_INN = "9725027246";

        private const string USProStampAltText = "USProStamp";
        private const string USProjectStampAltText = "USProjectStamp";

        private const string SignatureAltText = "USProSignature";

        private readonly ICollection<ContractAttachemntBookmark> contractAttachemntBookmarks =
        [
            new ContractAttachemntBookmark("АПС_1", "АПС_2", "Автоматическая пожарная сигнализация", "АПС"),
            new ContractAttachemntBookmark("АПТ_1", "АПТ_2", "Автоматическое пожаротушение", "АПТ"),
            new ContractAttachemntBookmark("АР_1", "АР_2", "Архитектурно-планировочные решения", "АР"),
            new ContractAttachemntBookmark("ВК_1", "ВК_2", "Водоснабжение и канализация", "ВК"),
            new ContractAttachemntBookmark("ВН_1", "ВН_2", "Видеонаблюдение", "В"),
            new ContractAttachemntBookmark("КР_1", "КР_2", "Конструктивные решения", "КР"),
            new ContractAttachemntBookmark("ОВиК_1", "ОВиК_2", "Отопление, вентиляция и кондиционирование", "ОВиК"),
            new ContractAttachemntBookmark("РАО_1", "РАО_2", "Расчет аварийного освещения", "РАО"),
            new ContractAttachemntBookmark("РО_1", "РО_2", "Расчет общего освещения", "РО"),
            new ContractAttachemntBookmark("СКС_1", "СКС_2", "Структурированная кабельная система", "СКС"),
            new ContractAttachemntBookmark("СКУД_1", "СКУД_2", "Система контроля и управление доступом", "СКУД"),
            new ContractAttachemntBookmark("СОУЭ_1", "СОУЭ_2", "Система оповещения и управления эвакуацией", "СОУЭ"),
            new ContractAttachemntBookmark("СС_1", "СС_2", "Сети связи", "СС"),
            new ContractAttachemntBookmark("ТХ_1", "ТХ_2", "Технологические решения", "ТХ"),
            new ContractAttachemntBookmark("ЭОМ_1", "ЭОМ_2", "Электрооборудование и электроосвещение", "ЭОМ"),
            new ContractAttachemntBookmark("СОСТ_1", "СОСТ_2", "Система охранно-тревожной сигнализации", "СОТС"),
            new ContractAttachemntBookmark("СПА_1", "СПА_2", "Система пожарной автоматики", "СПА"),
            new ContractAttachemntBookmark("ОТП_1", "ОТП_2", "Отопление", "ОТП"),
            new ContractAttachemntBookmark("ВПВ_1", "ВПВ_2", "Внутренний противопожарный водопровод", "ВПВ"),
            new ContractAttachemntBookmark("КМ_1", "КМ_2", "Конструкции металлические", "КМ"),
        ];

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

            CounterpartyDTO? executor;
            CounterpartyDTO? client;

            if (order.ParentId == null)
            {
                outputPath = Path.Combine(Path.GetTempPath(), $"Акт {order.Number!.Replace('/', '_')}-{order.Name}.docx");
                client = await counterpartyRepository.GetByIdAsync((Guid)order.CustomerId!);
                executor = await counterpartyRepository.GetByIdAsync((Guid)order.ExecutorId!);

                doc.Replace("{Address}", order.Address, true, true);
                doc.Replace("{NDS}", GetNDSDescription(order), true, true);
                doc.Replace("{AdditionalContractInfo}", "по ", true, true);
                doc.Replace("{ContractNumber}", order.Number, true, true);

                doc.Replace("{DSContractInfo}", ".1. договора ", true, true);
                doc.Replace("{ContractPoint}", "1.1 договора", true, true);
                doc.Replace("{ContractDate}", DateConverter.ConvertDateToString(order.StartDate), true, true);
            }
            else
            {
                outputPath = Path.Combine(Path.GetTempPath(), $"Акт ДС {order.Number!.Replace('/', '_')}-{order.Name}.docx");
                client = await counterpartyRepository.GetByIdAsync((Guid)order.ParentOrder!.CustomerId!);
                executor = await counterpartyRepository.GetByIdAsync((Guid)order.ParentOrder!.ExecutorId!);

                doc.Replace("{AdditionalContractInfo}", $"по Дополнительному соглашению №{order.Number}\n к", true, true);
                doc.Replace("{Address}", order.ParentOrder.Address, true, true);
                doc.Replace("{NDS}", GetNDSDescription(order.ParentOrder), true, true);
                doc.Replace("{ContractNumber}", order.ParentOrder.Number, true, true);

                doc.Replace("{DSContractInfo}", $" Дополнительного соглашения №{order.Number} от {DateConverter.ConvertDateToString(order.StartDate)} г. к", true, true);
                doc.Replace("{ContractPoint}", $"2. Дополнительного соглашения №{order.Number} от {DateConverter.ConvertDateToString(order.StartDate)} г. к Договору ", true, true);
                doc.Replace("{ContractDate}", DateConverter.ConvertDateToString(order.ParentOrder.StartDate), true, true);
            }

            doc.Replace("{Date}", DateConverter.ConvertDateToString(order.СompletionDate), true, true);
            doc.Replace("{Price}", string.Format("{0:N2}", order.Price), true, true);
            doc.Replace("{FullPrice}", DecimalConverter.ConvertDecimalToString(order.Price), true, true);

            if (order.SelectedServicesIds != null)
            {
                var servicesCollection = await serviceRepository.GetAllAsync();

                var selectedServices = order.SelectedServicesIds
                    .Select( serviceId => servicesCollection.FirstOrDefault( s => s.Id == serviceId ) )
                    .Where( service => service != null )
                    .ToList();

                var services = new StringBuilder();

                for (int i = 0; i < selectedServices.Count; i++)
                {
                    var service = selectedServices[i];

                    bool isLast = i == selectedServices.Count - 1;
                    string ending = isLast ? "." : ",";

                    services.Append( $"- Раздел «{service!.Name}»{ending}" );

                    if (!isLast)
                    {
                        services.AppendLine();
                    }
                }

                doc.Replace( "{Services}", services.ToString(), true, true );
            }

            var morpherService = new MorpherService();

            bool isClientIndividualEntrepreneur = IsIndividualEntrepreneur(client);

            string clientFullName = await morpherService.GetDeclensionAsync(
                client!.Director,
                MorpherService.RussianCase.Accusative);

            string clientPosition = GetDirectorPosition(client.DirectorPosition, false);

            doc.Replace("{ClientOrg}", client.Name, true, true);
            doc.Replace("{ClientNamedAs}", GetNamedAsDescription(client), true, true);
            doc.Replace("{ClientFullName}", clientFullName, true, true);
            doc.Replace("{ClientPosition}", clientPosition, true, true);
            doc.Replace("{ClientShortName}", await morpherService.GetShortNameAsync(client.Director, MorpherService.RussianCase.Nominative), true, true);

            doc.Replace("{ClientPreambleRepresentativeStart}", isClientIndividualEntrepreneur ? string.Empty : ", в лице ", true, true);
            doc.Replace("{ClientPreamblePosition}", isClientIndividualEntrepreneur ? string.Empty : clientPosition, true, true);
            doc.Replace("{ClientPreambleFullName}", isClientIndividualEntrepreneur ? string.Empty : clientFullName, true, true);
            doc.Replace("{ClientPreambleRepresentativeEnd}", isClientIndividualEntrepreneur ? string.Empty : ", действующего на основании Устава", true, true);

            doc.Replace("{ExecutorOrg}", executor!.Name, true, true);
            doc.Replace("{ExecutorFullName}", await morpherService.GetDeclensionAsync(executor.Director, MorpherService.RussianCase.Accusative), true, true);
            doc.Replace("{ExecutorPosition}", GetDirectorPosition(executor.DirectorPosition, false), true, true);
            doc.Replace("{ExecutorShortName}", await morpherService.GetShortNameAsync(executor.Director, MorpherService.RussianCase.Nominative), true, true);
            doc.Replace("{SRO}", GetSRO(executor.INN), true, true);

            try
            {
                outputPath = outputPath.Replace("\"", "");
                doc.SaveToFile(outputPath);
                ApplyExecutorStamp(outputPath, executor, stamp);

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

            doc.Replace("{ContractNumber}", order.Number, true, true);
            doc.Replace("{ContractDate}", $"{DateConverter.ConvertDateToString(DateTime.Now)}", true, true);
            doc.Replace("{Address}", order.Address, true, true);
            doc.Replace("{Square}", GetNumberDescription(order.Square, true), true, true);
            doc.Replace("{Deadline}", GetNumberDescription(order.Term), true, true);

            if (order.SelectedServicesIds != null)
            {
                var services = new StringBuilder();
                var servicesCollection = await serviceRepository.GetAllAsync();

                foreach (var serviceId in order.SelectedServicesIds)
                {
                    var service = servicesCollection.FirstOrDefault(s => s.Id == serviceId);

                    if (service != null)
                    {
                        services.Append($"- Раздел «{service.Name}»,\n");
                    }
                }

                services.Append("- Согласование проектной документации с Арендодателем.");

                doc.Replace("{Services}", services.ToString(), true, true);
            }

            var client = await counterpartyRepository.GetByIdAsync((Guid)order.CustomerId!);
            var executor = await counterpartyRepository.GetByIdAsync((Guid)order.ExecutorId!);

            var morpherService = new MorpherService();

            bool isClientIndividualEntrepreneur = IsIndividualEntrepreneur(client);

            string clientFullName = await morpherService.GetDeclensionAsync(
                client!.Director,
                MorpherService.RussianCase.Genitive);

            string clientPosition = GetDirectorPosition(client.DirectorPosition, false);

            doc.Replace("{ClientOrg}", client.Name, true, true);
            doc.Replace("{ClientNamedAs}", GetNamedAsDescription(client), true, true);
            doc.Replace("{ClientFullName}", clientFullName, true, true);
            doc.Replace("{ClientPosition}", clientPosition, true, true);
            doc.Replace("{ClientShortName}", await morpherService.GetShortNameAsync(client.Director, MorpherService.RussianCase.Nominative), true, true);
            doc.Replace("{ClientPositionI}", GetDirectorPosition(client.DirectorPosition, true), true, true);

            doc.Replace("{ClientPreambleRepresentativeStart}", isClientIndividualEntrepreneur ? string.Empty : ", в лице ", true, true);
            doc.Replace("{ClientPreamblePosition}", isClientIndividualEntrepreneur ? string.Empty : clientPosition, true, true);
            doc.Replace("{ClientPreambleFullName}", isClientIndividualEntrepreneur ? string.Empty : clientFullName, true, true);
            doc.Replace("{ClientPreambleRepresentativeEnd}", isClientIndividualEntrepreneur ? string.Empty : ", действующего на основании Устава", true, true);

            doc.Replace("{ExecutorOrg}", executor!.Name, true, true);
            doc.Replace("{ExecutorFullName}", await morpherService.GetDeclensionAsync(executor.Director, MorpherService.RussianCase.Genitive), true, true);
            doc.Replace("{ExecutorPosition}", GetDirectorPosition(executor.DirectorPosition, false), true, true);
            doc.Replace("{ExecutorPositionI}", GetDirectorPosition(executor.DirectorPosition, true), true, true);
            doc.Replace("{ExecutorShortName}", await morpherService.GetShortNameAsync(executor.Director, MorpherService.RussianCase.Nominative), true, true);
            doc.Replace("{SRO}", GetSRO(executor.INN), true, true);

            doc.Replace("{Price}", string.Format("{0:N2}", order.Price), true, true);
            doc.Replace("{FullPrice}", DecimalConverter.ConvertDecimalToString(order.Price), true, true);
            doc.Replace("{ExecutorDetails}", await CreateContragentDetails(order, client, executor, true), true, true);
            doc.Replace("{ClientDetails}", await CreateContragentDetails(order, client, executor, false), true, true);

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
                string postfix = NormalizeInn(executor.INN) != NormalizeInn(USProject_INN)
                    ? " (Уведомление о возможности применения УСН № 2490 от 03.12.2007 г.)"
                    : string.Empty;

                doc.Replace("{NDSNotExist}", $"НДС не облагается{postfix}", true, true);
                doc.Replace("{NDSExist}", string.Empty, true, true);
                doc.Replace("{NDSType}", "Без налога (НДС)", true, true);
                doc.Replace("{NDS}", "-", true, true);
            }

            if (order.PrepaymentPercent > 0)
            {
                var part = Math.Round((decimal)(order.Price! * order.PrepaymentPercent / 100), 2);
                var tax = Math.Round((part * order.NDS / (100 + order.NDS)), 2);
                var taxDescription = order.UsingNDS ? $", в том числе НДС {order.NDS}% {string.Format("{0:N2}", tax)} руб. ({DecimalConverter.ConvertDecimalToString(tax)})" : string.Empty;

                doc.Replace("{FirstPaymentPart}", $"\nВ течение 3 (Трех) банковских дней с момента подписания настоящего Договора Заказчик обязан произвести предоплату в размере {order.PrepaymentPercent} % от стоимости работ, " +
                    $"указанных в п. 4.1 настоящего Договора, что составляет {string.Format("{0:N2}", part)} руб. ({DecimalConverter.ConvertDecimalToString(part)}){taxDescription}.", true, true);
            }
            else
            {
                doc.Replace("{FirstPaymentPart}", string.Empty, true, true);
            }

            if (order.ExecutionPercent > 0)
            {
                var part = Math.Round((decimal)(order.Price! * order.ExecutionPercent / 100), 2);
                var tax = Math.Round((part * order.NDS / (100 + order.NDS)), 2);
                var taxDescription = order.UsingNDS ? $", в том числе НДС {order.NDS}% {string.Format("{0:N2}", tax)} руб. ({DecimalConverter.ConvertDecimalToString(tax)})" : string.Empty;

                doc.Replace("{SecondPaymentPart}", $"\nВторую часть в размере {order.ExecutionPercent} % Заказчик должен внести в течении 2 (Двух) банковских дней после полного выполнения подрядчиком всех разделов проектной документации указанных в п. 1.2., " +
                    $"что составляет {string.Format("{0:N2}", part)} руб. ({DecimalConverter.ConvertDecimalToString(part)}){taxDescription}.", true, true);
            }
            else
            {
                doc.Replace("{SecondPaymentPart}", string.Empty, true, true);
            }

            if (order.ApprovalPercent > 0)
            {
                var part = Math.Round((decimal)(order.Price! * order.ApprovalPercent / 100), 2);
                var tax = Math.Round((part * order.NDS / (100 + order.NDS)), 2);
                var taxDescription = order.UsingNDS ? $", в том числе НДС {order.NDS}% {string.Format("{0:N2}", tax)} руб. ({DecimalConverter.ConvertDecimalToString(tax)})" : string.Empty;

                doc.Replace("{ThirdPaymentPart}", $"\nОставшиеся {order.ApprovalPercent} % от стоимости работ, указанных в п. 4.1. настоящего Договора, " +
                    $"что составляет {string.Format("{0:N2}", part)} руб. ({DecimalConverter.ConvertDecimalToString(part)}){taxDescription}, " +
                    $"Заказчик вносит в течение 3 (Трех) банковских дней после согласования проектной документации с арендодателем.", true, true);
            }
            else
            {
                doc.Replace("{ThirdPaymentPart}", string.Empty, true, true);
            }

            try
            {
                outputPath = outputPath.Replace("\"", "");
                doc.SaveToFile(outputPath);
                ApplyExecutorStamp(outputPath, executor, stamp);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(outputPath) { UseShellExecute = true });
            }
            catch (Exception)
            {
                throw new Exception("Невозможно сохранить договор. Вероятно, он уже открыт. Закройте документ и попробуйте снова");
            }

            await CreateContractAttachments(order);
        }

        private async Task CreateContractAttachments(OrderDTO order)
        {
            string templatePath = Path.Combine("Templates", "ContractAttachment.docx");
            string outputPath = Path.Combine(Path.GetTempPath(), $"Приложение к договору {order.Number!.Replace('/', '_')}-{order.Name}.docx");

            Document doc = new();
            try
            {
                doc.LoadFromFile(templatePath);
            }
            catch (Exception)
            {
                throw new Exception("Невозможно открыть шаблон документа. Вероятно, он отсутствует в папке Templates.");
            }

            doc.Replace("{ContractNumber}", order.Number, true, true);
            doc.Replace("{ContractDate}", $"{DateConverter.ConvertDateToString(order.StartDate)}", true, true);

            if (order.SelectedServicesIds != null)
            {
                var allServices = await serviceRepository.GetAllAsync();
                var usedService = allServices.Where(x => order.SelectedServicesIds.Contains(x.Id!.Value)).ToList();
                var toRemoveAbbreviations = allServices.Except(usedService).Select(x => x.Abbreviation).ToList();
                var toRemoveBookmarks = contractAttachemntBookmarks.Where(x => toRemoveAbbreviations.Contains(x.Abbreviation)).ToList();

                if (toRemoveBookmarks.Count > 0)
                {
                    foreach (var bookmark in toRemoveBookmarks)
                    {
                        var firstAppBookmark = doc.Bookmarks[bookmark.FirstAppBookmark];
                        var secondAppBookmark = doc.Bookmarks[bookmark.SecondAppBookmark];

                        BookmarksNavigator navigator = new(doc);
                        if (firstAppBookmark != null)
                        {
                            navigator.MoveToBookmark(firstAppBookmark.Name, true, true);
                            navigator.DeleteBookmarkContent(true);
                        }

                        if (secondAppBookmark != null)
                        {
                            navigator.MoveToBookmark(secondAppBookmark.Name, true, true);
                            navigator.DeleteBookmarkContent(true);
                        }
                    }
                }
            }

            var client = await counterpartyRepository.GetByIdAsync((Guid)order.CustomerId!);
            var executor = await counterpartyRepository.GetByIdAsync((Guid)order.ExecutorId!);

            var morpherService = new MorpherService();

            doc.Replace("{ClientShortName}", await morpherService.GetShortNameAsync(client!.Director, MorpherService.RussianCase.Nominative), true, true);
            doc.Replace("{ClientPosition}", GetDirectorPosition(client.DirectorPosition, true), true, true);

            doc.Replace("{ExecutorPosition}", GetDirectorPosition(executor!.DirectorPosition, true), true, true);
            doc.Replace("{ExecutorShortName}", await morpherService.GetShortNameAsync(executor.Director, MorpherService.RussianCase.Nominative), true, true);

            try
            {
                outputPath = outputPath.Replace("\"", "");
                doc.SaveToFile(outputPath);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(outputPath) { UseShellExecute = true });
            }
            catch (Exception)
            {
                throw new Exception("Невозможно сохранить приложение к договору. Вероятно, оно уже открыто. Закройте документ и попробуйте снова");
            }
        }

        public async Task CreateAdditionalContractAsync(OrderDTO order, bool stamp)
        {
            string templatePath = Path.Combine("Templates", "AdditionalContract.docx");
            string outputPath = Path.Combine(Path.GetTempPath(), $"ДС {order.Number!.Replace('/', '_')} к договору {order.ParentOrder!.Number!.Replace('/', '_')}-{order.Name}.docx");

            Document doc = new();
            try
            {
                doc.LoadFromFile(templatePath);
            }
            catch (Exception)
            {
                throw new Exception("Невозможно открыть шаблон документа. Вероятно, он отсутствует в папке Templates.");
            }

            doc.Replace("{Number}", order.Number, true, true);
            doc.Replace("{ContractNumber}", order.ParentOrder.Number, true, true);
            doc.Replace("{ContractDate}", $"{DateConverter.ConvertDateToString(order.ParentOrder.StartDate)}", true, true);
            doc.Replace("{Date}", $"{DateConverter.ConvertDateToString(DateTime.Now)}", true, true);
            doc.Replace("{Address}", order.ParentOrder.Address, true, true);
            doc.Replace("{Square}", GetNumberDescription(order.ParentOrder.Square, true), true, true);
            doc.Replace("{Deadline}", GetNumberDescription(order.Term), true, true);

            if (order.SelectedServicesIds != null)
            {
                var services = new StringBuilder();
                var servicesCollection = await serviceRepository.GetAllAsync();

                foreach (var serviceId in order.SelectedServicesIds)
                {
                    var service = servicesCollection.FirstOrDefault(s => s.Id == serviceId);

                    if (service != null)
                    {
                        services.Append($"- Раздел «{service.Name}»,\n");
                    }
                }

                doc.Replace("{Services}", services.ToString(), true, true);
            }

            var client = await counterpartyRepository.GetByIdAsync((Guid)order.ParentOrder!.CustomerId!);
            var executor = await counterpartyRepository.GetByIdAsync((Guid)order.ParentOrder!.ExecutorId!);

            var morpherService = new MorpherService();

            bool isClientIndividualEntrepreneur = IsIndividualEntrepreneur(client);

            string clientFullName = await morpherService.GetDeclensionAsync(
                client!.Director,
                MorpherService.RussianCase.Accusative);

            string clientPosition = GetDirectorPosition(client.DirectorPosition, false);

            doc.Replace("{ClientOrg}", client.Name, true, true);
            doc.Replace("{ClientNamedAs}", GetNamedAsDescription(client), true, true);
            doc.Replace("{ClientFullName}", clientFullName, true, true);
            doc.Replace("{ClientPosition}", clientPosition, true, true);
            doc.Replace("{ClientShortName}", await morpherService.GetShortNameAsync(client.Director, MorpherService.RussianCase.Nominative), true, true);

            doc.Replace("{ClientPreambleRepresentativeStart}", isClientIndividualEntrepreneur ? string.Empty : ", в лице ", true, true);
            doc.Replace("{ClientPreamblePosition}", isClientIndividualEntrepreneur ? string.Empty : clientPosition, true, true);
            doc.Replace("{ClientPreambleFullName}", isClientIndividualEntrepreneur ? string.Empty : clientFullName, true, true);
            doc.Replace("{ClientPreambleRepresentativeEnd}", isClientIndividualEntrepreneur ? string.Empty : ", действующего на основании Устава", true, true);

            doc.Replace("{ExecutorOrg}", executor!.Name, true, true);
            doc.Replace("{ExecutorFullName}", await morpherService.GetDeclensionAsync(executor.Director, MorpherService.RussianCase.Accusative), true, true);
            doc.Replace("{ExecutorPosition}", GetDirectorPosition(executor.DirectorPosition, false), true, true);
            doc.Replace("{ExecutorShortName}", await morpherService.GetShortNameAsync(executor.Director, MorpherService.RussianCase.Nominative), true, true);
            doc.Replace("{SRO}", GetSRO(executor.INN), true, true);

            doc.Replace("{Price}", string.Format("{0:N2}", order.Price), true, true);
            doc.Replace("{FullPrice}", DecimalConverter.ConvertDecimalToString(order.Price), true, true);

            if (order.ParentOrder!.UsingNDS && order.ParentOrder!.NDS > 0)
            {
                var tax = Math.Round((decimal)(order.Price! * order.ParentOrder!.NDS / (100 + order.ParentOrder!.NDS)), 2);
                doc.Replace("{NDSType}", $"В том числе НДС {order.ParentOrder!.NDS}%", true, true);
                doc.Replace("{NDS}", string.Format("{0:N2}", tax), true, true);
                doc.Replace("{NDSNotExist}", string.Empty, true, true);
                doc.Replace("{NDSExist}", $"В том числе НДС {order.ParentOrder!.NDS}% {string.Format("{0:N2}", tax)} ({DecimalConverter.ConvertDecimalToString(tax)}) рублей", true, true);
            }
            else
            {
                string postfix = NormalizeInn(executor.INN) != NormalizeInn(USProject_INN)
                    ? " (Уведомление о возможности применения УСН № 2490 от 03.12.2007 г.)"
                    : string.Empty;

                doc.Replace("{NDSNotExist}", $"НДС не облагается{postfix}", true, true);
                doc.Replace("{NDSExist}", string.Empty, true, true);
                doc.Replace("{NDSType}", "Без налога (НДС)", true, true);
                doc.Replace("{NDS}", "-", true, true);
            }

            var calculations = new StringBuilder();

            if (order.PrepaymentPercent != null && order.PrepaymentPercent > 0 && order.ExecutionPercent != null && order.ExecutionPercent > 0)
            {
                calculations.Append($"В течение 3 (Трех) банковских дней с момента подписания настоящего Дополнительного соглашения Заказчик обязан произвести предоплату в размере {GetNumberDescription(order.PrepaymentPercent)} % от стоимости работ, указанных в п.3 настоящего Дополнительного соглашения. ");
                calculations.Append($"Вторую часть в размере {GetNumberDescription(order.ExecutionPercent)} % заказчик должен внести в течении двух банковских дней после полного выполнения подрядчиком всех разделов проектной документации указанных в п. 2. ");
            }

            if (order.PrepaymentPercent > 0 && (order.ExecutionPercent == 0 || order.ExecutionPercent == null))
            {
                calculations.Append($"В течение 3 (Трех) банковских дней с момента подписания настоящего Дополнительного соглашения Заказчик обязан произвести предоплату в размере {GetNumberDescription(order.PrepaymentPercent)} % от стоимости работ, указанных в п.3 настоящего Дополнительного соглашения.");
            }

            if ((order.PrepaymentPercent == null || order.PrepaymentPercent == 0) && order.ExecutionPercent > 0)
            {
                calculations.Append($"В течение 3 (Трех) банковских дней с момента подписания настоящего Дополнительного соглашения Заказчик обязан произвести оплату в размере {GetNumberDescription(order.ExecutionPercent)} % от стоимости работ, указанных в п.3 настоящего Дополнительного соглашения.");
            }

            doc.Replace("{Calculations}", calculations.ToString(), true, true);

            try
            {
                outputPath = outputPath.Replace("\"", "");
                doc.SaveToFile(outputPath);

                ApplyExecutorStamp(outputPath, executor, stamp);

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

            doc.Replace("{Number}", order.Number, true, true);
            doc.Replace("{Address}", order.Address, true, true);
            doc.Replace("{Service}", order.AdditionalService, true, true);
            doc.Replace("{Square}", order.Square.ToString(), true, true);
            doc.Replace("{Deadline}", GetNumberDescription(order.Term), true, true);
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
                doc.Replace("{ClientINN}", client.INN != null ? client.INN : string.Empty, true, true);
                doc.Replace("{ClientKPP}", client.KPP != null ? client.KPP : string.Empty, true, true);
                doc.Replace("{Email}", order.Email != null ? order.Email : string.Empty, true, true);
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
                outputPath = outputPath.Replace("\"", "");
                doc.SaveToFile(outputPath);
                ApplyExecutorStamp(outputPath, executor, stamp);

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
                    outputPath = Path.Combine(Path.GetTempPath(), $"Счет (согласование) {order.ApprovalBillNumber} от {order.ApprovalBillDate:dd.MM.yyyy} {order.Number!.Replace('/', '_')}-{order.Name}.docx");
                    number = $"{order.ApprovalBillNumber} от {DateConverter.ConvertDateToString(order.ApprovalBillDate)} г.";
                    price = (decimal)(order.Price! * order.ApprovalPercent! / 100);
                    billType = $"Оплата ({order.ApprovalPercent}%)";
                    break;
            }

            doc.Replace("{Number}", number, true, true);

            doc.Replace("{Price}", string.Format("{0:N2}", price), true, true);
            doc.Replace("{FullPrice}", DecimalConverter.ConvertDecimalToString(price), true, true);
            doc.Replace("{PayType}", billType, true, true);

            CounterpartyDTO? executor;
            CounterpartyDTO? client;

            if (order.ParentId == null)
            {
                doc.Replace("{AdditionalOrderReason}", string.Empty, true, true);
                doc.Replace("{Contract}", $"{order.Number} от {order.StartDate:dd.MM.yyyy} г.", true, true);
                doc.Replace("{Object}", order.Name, true, true);
                doc.Replace("{AdditionalOrder}", string.Empty, true, true);

                if (order.UsingNDS && order.NDS > 0)
                {
                    var tax = Math.Round(price * order.NDS / (100 + order.NDS), 2);
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
                doc.Replace("{AdditionalOrderReason}", $"Дополнительное соглашение №{order.Number} от {order.StartDate:dd.MM.yyyy} г. к договору ", true, true);
                doc.Replace("{Contract}", $"{order.ParentOrder!.Number} от {order.ParentOrder!.StartDate:dd.MM.yyyy} г.", true, true);
                doc.Replace("{Object}", order.ParentOrder!.Name, true, true);
                doc.Replace("{AdditionalOrder}", $"доп. соглашению № {order.Number} от {order.StartDate:dd.MM.yyyy} по ", true, true);

                if (order.ParentOrder!.UsingNDS && order.ParentOrder!.NDS > 0)
                {
                    var tax = Math.Round(price * order.ParentOrder!.NDS / (100 + order.ParentOrder!.NDS), 2);
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

            // Добавление списка услуг при наличии.
            if (order.SelectedServicesIds != null)
            {
                var services = new List<string>();
                var servicesCollection = await serviceRepository.GetAllAsync();

                foreach (var serviceId in order.SelectedServicesIds)
                {
                    Service? service = servicesCollection.FirstOrDefault(s => s.Id == serviceId);

                    if (service != null)
                    {
                        services.Add(service.Abbreviation);
                    }
                }

                doc.Replace("{Services}", string.Join(", ", services), true, true);
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
                outputPath = outputPath.Replace("\"", "");
                doc.SaveToFile(outputPath);
                ApplyExecutorStamp(outputPath, executor, stamp);

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

            try
            {
                outputPath = outputPath.Replace("\"", "");
                FileInfo outputFile = new(outputPath);
                package.SaveAs(outputFile);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(outputPath) { UseShellExecute = true });
            }
            catch (Exception)
            {
                throw new Exception("Невозможно сохранить УПД. Вероятно, он уже открыт. Закройте документ и попробуйте снова");
            }
        }

        /// <summary>
        /// Создать реквизиты контрагента.
        /// </summary>
        /// <param name="order">Заказ.</param>
        /// <param name="client">Заказчик.</param>
        /// <param name="executor">Исполнитель.</param>
        /// <param name="isExecutor">Признак реквизитов исполнителя.</param>
        /// <returns>Строка с реквизитами контрагента.</returns>
        private static async Task<string> CreateContragentDetails(OrderDTO order, CounterpartyDTO? client, CounterpartyDTO? executor, bool isExecutor)
        {
            StringBuilder details = new();
            var morpherService = new MorpherService();

            if (isExecutor && executor != null)
            {
                details.AppendLine(executor.Name);
                details.AppendLine($"ИНН/КПП {executor.INN}/{executor.KPP}");
                details.AppendLine($"Юридический адрес: {executor.Address}");
                details.AppendLine("Банковские реквизиты:");
                details.AppendLine($"р/с: {executor.PaymentAccount}");
                details.AppendLine($"в {executor.Bank}");
                details.AppendLine($"к/с: {executor.CorrAccount}");
                details.AppendLine($"БИК {executor.BIK}");
                details.AppendLine("(домен @usproject.ru)");
                details.AppendLine();
                details.AppendLine(GetDirectorPosition(executor.DirectorPosition, true));
                details.AppendLine($"/{await morpherService.GetShortNameAsync(executor.Director, MorpherService.RussianCase.Nominative)}/");
                details.AppendLine();
                details.AppendLine("М.П.");
            }

            if (!isExecutor && client != null)
            {
                details.AppendLine(client.Name);
                details.AppendLine($"ИНН/КПП {client.INN}/{client.KPP}");
                details.AppendLine($"Юридический адрес: {client.Address}");
                details.AppendLine("Банковские реквизиты:");
                details.AppendLine($"р/с: {client.PaymentAccount}");
                details.AppendLine($"в {client.Bank}");
                details.AppendLine($"к/с: {client.CorrAccount}");
                details.AppendLine($"БИК {client.BIK}");
                details.AppendLine($"E-mail {order.Email}");
                details.AppendLine($"Телефон {order.Phone}");
                details.AppendLine();
                details.AppendLine(GetDirectorPosition(client.DirectorPosition, true));
                details.AppendLine($"/{await morpherService.GetShortNameAsync(client.Director, MorpherService.RussianCase.Nominative)}/");
                details.AppendLine();
                details.AppendLine("М.П.");
            }

            return details.ToString();
        }

        /// <summary>
        /// Получить описание числа.
        /// </summary>
        /// <param name="number">Число.</param>
        /// <param name="inv">Признак именительного падежа.</param>
        /// <returns>Описание числа.</returns>
        private static string GetNumberDescription(int? number, bool inv = false)
        {
            if (number == null || number == 0)
            {
                return "нуля";
            }

            int term = (int)number;

            string[] units;
            string[] tens;
            string[] hundreds;
            string[] teens;

            if (inv)
            {
                units = ["", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять"];
                teens = ["десять", "одиннадцать", "двенадцать", "тринадцать", "четырнадцать", "пятнадцать", "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать"];
                tens = ["", "", "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто"];
                hundreds = ["", "сто", "двести", "триста", "четыреста", "пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот"];
            }
            else
            {
                units = ["", "одного", "двух", "трех", "четырех", "пяти", "шести", "семи", "восьми", "девяти"];
                teens = ["десяти", "одиннадцати", "двенадцати", "тринадцати", "четырнадцати", "пятнадцати", "шестнадцати", "семнадцати", "восемнадцати", "девятнадцати"];
                tens = ["", "", "двадцати", "тридцати", "сорока", "пятидесяти", "шестидесяти", "семидесяти", "восьмидесяти", "девяноста"];
                hundreds = ["", "ста", "двухсот", "трехсот", "четырехсот", "пятисот", "шестисот", "семисот", "восьмисот", "девятисот"];
            }

            var parts = new List<string>();

            if (number >= 100)
            {
                int hundredPart = (int)number / 100;
                parts.Add(hundreds[hundredPart]);
                number %= 100;
            }

            if (number >= 20)
            {
                int tensPart = (int)number / 10;
                parts.Add(tens[tensPart]);
                number %= 10;
            }

            if (number >= 10)
            {
                parts.Add(teens[(int)number - 10]);
                number = 0;
            }

            if (number > 0)
            {
                parts.Add(units[(int)number]);
            }

            return $"{term} ({string.Join(" ", parts)})";
        }

        /// <summary>
        /// Получить описание НДС.
        /// </summary>
        /// <param name="order">Заказ.</param>
        /// <returns>Описание НДС.</returns>
        private static string GetNDSDescription(OrderDTO order)
        {
            const string notNDS = "НДС не облагается (Уведомление о возможности применения УСН № 2490 от 03.12.2007 г.)";

            if (order.UsingNDS && order.NDS > 0)
            {
                var tax = Math.Round((decimal)order.Price! * order.NDS / (100 + order.NDS), 2);
                return $"в том числе НДС {order.NDS}% ({string.Format("{0:N2}", tax)} рублей)";
            }

            return notNDS;
        }

        /// <summary>
        /// Получить СРО в зависимости от ИНН.
        /// </summary>
        /// <param name="inn">ИНН контрагента.</param>
        /// <returns>Описание членства в СРО.</returns>
        private static string GetSRO(string? inn)
        {
            return NormalizeInn(inn) switch
            {
                USPro_INN => "член саморегулируемой организации Ассоциация проектировщиков саморегулируемая организация «Объединение проектных организаций «ЭкспертПроект» СРО-П-182-02042013 с 12.01.2018г., ",
                USProject_INN => "член саморегулируемой организации Ассоциация организаций, осуществляющих проектирование энергетических объектов «ЭНЕРГОПРОЕКТ» СРО-П-068- 02122009 с 28.01.2020г., ",
                _ => string.Empty,
            };
        }

        /// <summary>
        /// Применяет печать и подпись исполнителя к сформированному Word-документу.
        /// </summary>
        /// <param name="documentPath">Путь к сформированному документу.</param>
        /// <param name="executor">Исполнитель.</param>
        /// <param name="stamp">Признак необходимости печати и подписи.</param>
        private static void ApplyExecutorStamp(string documentPath, CounterpartyDTO? executor, bool stamp)
        {
            string executorInn = NormalizeInn(executor?.INN);

            if (!stamp)
            {
                RemoveExecutorSigningImages(documentPath);
                return;
            }

            if (executorInn == NormalizeInn(USPro_INN))
            {
                // Исполнитель USPro - оставляем правую печать и общую подпись.
                RemovePicturesByAltText(documentPath, USProjectStampAltText);
                return;
            }

            if (executorInn == NormalizeInn(USProject_INN))
            {
                // Исполнитель USProject - оставляем левую печать и общую подпись.
                RemovePicturesByAltText(documentPath, USProStampAltText);
                return;
            }

            // Неизвестный исполнитель - убираем все подписантские изображения.
            RemoveExecutorSigningImages(documentPath);
        }

        /// <summary>
        /// Удаляет печати и подпись исполнителя.
        /// </summary>
        /// <param name="documentPath">Путь к сформированному документу.</param>
        private static void RemoveExecutorSigningImages(string documentPath)
        {
            RemovePicturesByAltText(documentPath, USProStampAltText);
            RemovePicturesByAltText(documentPath, USProjectStampAltText);
            RemovePicturesByAltText(documentPath, SignatureAltText);
        }

        /// <summary>
        /// Удаляет изображения из Word-документа по Alt Text.
        /// </summary>
        /// <param name="documentPath">Путь к Word-документу.</param>
        /// <param name="altText">Alt Text изображения.</param>
        private static void RemovePicturesByAltText(string documentPath, string altText)
        {
            using WordprocessingDocument document = WordprocessingDocument.Open(documentPath, true);

            MainDocumentPart? mainPart = document.MainDocumentPart;

            if (mainPart == null)
            {
                return;
            }

            RemovePicturesByAltText(mainPart.Document, altText);
            mainPart.Document.Save();

            foreach (HeaderPart headerPart in mainPart.HeaderParts)
            {
                RemovePicturesByAltText(headerPart.Header, altText);
                headerPart.Header.Save();
            }

            foreach (FooterPart footerPart in mainPart.FooterParts)
            {
                RemovePicturesByAltText(footerPart.Footer, altText);
                footerPart.Footer.Save();
            }
        }

        /// <summary>
        /// Удаляет изображения из части Word-документа по Alt Text.
        /// </summary>
        /// <param name="root">Корневой элемент части документа.</param>
        /// <param name="altText">Alt Text изображения.</param>
        private static void RemovePicturesByAltText(OpenXmlCompositeElement root, string altText)
        {
            List<W.Drawing> drawings = root
                .Descendants<W.Drawing>()
                .Where(drawing => HasAltText(drawing, altText))
                .ToList();

            foreach (W.Drawing drawing in drawings)
            {
                drawing.Remove();
            }
        }

        /// <summary>
        /// Проверяет, имеет ли изображение указанный Alt Text.
        /// </summary>
        /// <param name="drawing">Изображение Word.</param>
        /// <param name="altText">Alt Text.</param>
        /// <returns>Признак совпадения.</returns>
        private static bool HasAltText(W.Drawing drawing, string altText)
        {
            return drawing
                .Descendants<WP.DocProperties>()
                .Any(properties =>
                    string.Equals(properties.Title?.Value, altText, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(properties.Description?.Value, altText, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(properties.Name?.Value, altText, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Нормализует ИНН, оставляя только цифры.
        /// </summary>
        /// <param name="inn">ИНН.</param>
        /// <returns>Нормализованный ИНН.</returns>
        private static string NormalizeInn(string? inn)
        {
            return new string((inn ?? string.Empty)
                .Where(char.IsDigit)
                .ToArray());
        }

        /// <summary>
        /// Возвращает слово "именуемый" или "именуемое" для преамбулы договора.
        /// </summary>
        /// <param name="counterparty">Контрагент.</param>
        /// <returns>Форма слова для преамбулы договора.</returns>
        private static string GetNamedAsDescription(CounterpartyDTO? counterparty)
        {
            return IsIndividualEntrepreneur(counterparty)
                ? "именуемый"
                : "именуемое";
        }

        /// <summary>
        /// Определяет, является ли контрагент индивидуальным предпринимателем.
        /// </summary>
        /// <param name="counterparty">Контрагент.</param>
        /// <returns>Признак индивидуального предпринимателя.</returns>
        private static bool IsIndividualEntrepreneur(CounterpartyDTO? counterparty)
        {
            return NormalizeInn(counterparty?.INN).Length == 12;
        }

        /// <summary>
        /// Получить должность директора.
        /// </summary>
        /// <param name="directorPosition">Должность.</param>
        /// <param name="nominative">Именительный падеж.</param>
        /// <returns>Должность директора.</returns>
        /// <exception cref="NotImplementedException">Неизвестная должность директора.</exception>
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
                _ => throw new NotImplementedException(),
            };
        }
    }

    public class ContractAttachemntBookmark(string fisrtBookmark, string secondBookmark, string name, string abbr)
    {
        public string FirstAppBookmark { get; } = fisrtBookmark;
        public string SecondAppBookmark { get; } = secondBookmark;
        public string Name { get; } = name;
        public string Abbreviation { get; } = abbr;
    }
}
