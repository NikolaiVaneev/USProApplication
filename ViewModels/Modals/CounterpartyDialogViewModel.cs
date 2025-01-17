using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using USProApplication.DataBase.Entities;
using USProApplication.Models;
using USProApplication.Models.API;
using USProApplication.Models.Repositories;
using USProApplication.Utils;

namespace USProApplication.ViewModels.Modals;

public class CounterpartyDialogViewModel(ICounterpartyRepository counterpartyRepository) : ReactiveObject
{
    [Reactive] public CounterpartyDTO? Counterparty { get; set; }
    [Reactive] public bool IsBankInfoLoad { get; set; }
    [Reactive] public bool IsCounterpartyInfoLoad { get; set; }
    public string? PreINN { get; set; } 

    public event Action<CounterpartyDTO?>? OnSave;

    public ObservableCollection<string> DirectorPositionDescriptions { get; } =
        new ObservableCollection<string>(EnumExtensions.GetDescriptions<DirectorPositions>());

    private AsyncCommand? apply;
    public AsyncCommand Apply => apply ??= new AsyncCommand(async () =>
    {
        if (PreINN != Counterparty!.INN)
        {
            var findedOrgName = await counterpartyRepository.CheckCounterpartyExistAsync(Counterparty!.INN);
            if (!string.IsNullOrWhiteSpace(findedOrgName))
            {
                var result = MessageBox.Show($"С данным ИНН уже имеется контрагент \"{findedOrgName}\". Вы действительно хотите сохранить запись?", "Подтверждение сохранения", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
        }

        OnSave?.Invoke(Counterparty);
    }, () => !string.IsNullOrWhiteSpace(Counterparty?.Name));

    private AsyncCommand? findСounterpartyInfo;
    public AsyncCommand FindСounterpartyInfoAsync => findСounterpartyInfo ??= new AsyncCommand(async () =>
    {
        if (string.IsNullOrWhiteSpace(Counterparty?.INN))
            return;

        IsCounterpartyInfoLoad = true;

        try
        {
            // Создаем клиент для HTTP-запроса
            using var httpClient = new HttpClient();

            // Формируем URL запроса
            string url = $"https://api.ofdata.ru/v2/company?key=dPaSrxqECeDOIYdB&inn={Counterparty.INN}";

            // Выполняем запрос и парсим результат
            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                ShowWarning("Не удалось получить данные о контрагенте. Проверьте соединение или попробуйте позже.");
                return;
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            var companyData = JsonConvert.DeserializeObject<CompanyApiResponse>(responseContent);

            if (companyData?.Data!.ИНН == null)
            {
                ShowWarning("Данные о контрагенте не найдены.");
                return;
            }

            // Проверяем существующие значения
            bool hasExistingValues = !string.IsNullOrWhiteSpace(Counterparty.Name) ||
                                     !string.IsNullOrWhiteSpace(Counterparty.OGRN) ||
                                     !string.IsNullOrWhiteSpace(Counterparty.KPP) ||
                                     !string.IsNullOrWhiteSpace(Counterparty.Address) ||
                                     !string.IsNullOrWhiteSpace(Counterparty.Director);

            // Предупреждение об изменении существующих данных
            if (hasExistingValues)
            {
                bool userConfirmed = ShowConfirmationDialog("Данные о контрагенте уже заполнены. Вы уверены, что хотите их перезаписать?");
                if (!userConfirmed)
                    return;
            }

            // Заполняем свойства
            Counterparty.Name = companyData.Data.НаимСокр;
            Counterparty.OGRN = companyData.Data.ОГРН;
            Counterparty.KPP = companyData.Data.КПП;
            Counterparty.Address = companyData.Data.ЮрАдрес?.АдресРФ;

            var director = companyData.Data.Руковод?.FirstOrDefault();

            if (director != null)
            {
                Counterparty.Director = director.ФИО;
                string directorPositionName = director.НаимДолжн ?? string.Empty;
                Counterparty.DirectorPosition = MapDirectorPosition(directorPositionName);
            }
            this.RaisePropertyChanged(nameof(Counterparty));
        }
        catch (Exception ex)
        {
            ShowWarning($"Ошибка при получении данных о контрагенте: {ex.Message}");
        }
        finally
        {
            IsCounterpartyInfoLoad = false;
        }
    }, () => !string.IsNullOrWhiteSpace(Counterparty?.INN) && !IsCounterpartyInfoLoad);

    private AsyncCommand? findBankInfo;
    public AsyncCommand FindBankInfoAsync => findBankInfo ??= new AsyncCommand(async () =>
    {
        if (string.IsNullOrWhiteSpace(Counterparty?.BIK))
            return;

        IsBankInfoLoad = true;

        try
        {
            using var httpClient = new HttpClient();

            // Формируем URL запроса
            string url = $"https://api.ofdata.ru/v2/bank?key=dPaSrxqECeDOIYdB&bic={Counterparty.BIK}";

            // Выполняем запрос и парсим результат
            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                // Ошибка подключения или ответа API
                ShowWarning("Не удалось получить данные о банке. Проверьте соединение или попробуйте позже.");
                return;
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            var bankData = JsonConvert.DeserializeObject<BankApiResponse>(responseContent);

            if (bankData?.Data!.БИК == null)
            {
                ShowWarning("Данные о банке не найдены.");
                return;
            }

            // Проверяем существующие значения
            bool hasExistingValues = !string.IsNullOrWhiteSpace(Counterparty.Bank) || !string.IsNullOrWhiteSpace(Counterparty.CorrAccount);

            // Предупреждение об изменении существующих данных
            if (hasExistingValues)
            {
                bool userConfirmed = ShowConfirmationDialog("Данные о банке уже заполнены. Вы уверены, что хотите их перезаписать?");
                if (!userConfirmed)
                    return;
            }

            // Заполняем свойства
            Counterparty.Bank = $"{bankData.Data.Наим} {bankData.Data.Адрес}";
            Counterparty.CorrAccount = bankData?.Data?.КорСчет?.Номер;
            this.RaisePropertyChanged(nameof(Counterparty));
        }
        catch (Exception ex)
        {
            ShowWarning($"Ошибка при получении данных о банке: {ex.Message}");
        }
        finally
        {
            IsBankInfoLoad = false;
        }
    }, () => !string.IsNullOrWhiteSpace(Counterparty?.BIK) && !IsBankInfoLoad);

    private static void ShowWarning(string message)
    {
        MessageBox.Show(message, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private static bool ShowConfirmationDialog(string message)
    {
        var result = MessageBox.Show(message, "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        return result == MessageBoxResult.Yes;
    }

    private static DirectorPositions MapDirectorPosition(string positionName)
    {
        if (string.IsNullOrWhiteSpace(positionName))
            return DirectorPositions.None;

        return Enum.GetValues(typeof(DirectorPositions))
                   .Cast<DirectorPositions>()
                   .FirstOrDefault(pos => string.Equals(pos.GetDescription(), positionName, StringComparison.OrdinalIgnoreCase));
    }
}
