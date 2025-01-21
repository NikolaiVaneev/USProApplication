using Dadata;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Windows;
using USProApplication.DataBase.Entities;
using USProApplication.Models;
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
            var token = "ebd243812ca0e027a95fe554097d95acd5f9d822";
            var api = new SuggestClientAsync(token);
            var result = await api.FindParty(Counterparty?.INN);

            if (result.suggestions.Count > 0)
            {
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

                var companyData = result.suggestions[0];

                // Заполняем свойства
                Counterparty.Name = companyData.data.name.full_with_opf;
                Counterparty.OGRN = companyData.data.ogrn;
                Counterparty.KPP = companyData.data.kpp;
                Counterparty.Address = companyData.data.address.value;
                Counterparty.Director = companyData.data.management.name;
                Counterparty.DirectorPosition = MapDirectorPosition(companyData.data.management.post);

                this.RaisePropertyChanged(nameof(Counterparty));
            }
            else
            {
                ShowWarning("Данные о контрагенте не найдены.");
                return;
            }
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
            var token = "ebd243812ca0e027a95fe554097d95acd5f9d822";
            var api = new SuggestClientAsync(token);
            var result = await api.FindBank(Counterparty?.BIK);

            if (result.suggestions.Count > 0)
            {
                // Проверяем существующие значения
                bool hasExistingValues = !string.IsNullOrWhiteSpace(Counterparty.Bank) || !string.IsNullOrWhiteSpace(Counterparty.CorrAccount);

                // Предупреждение об изменении существующих данных
                if (hasExistingValues)
                {
                    bool userConfirmed = ShowConfirmationDialog("Данные о банке уже заполнены. Вы уверены, что хотите их перезаписать?");
                    if (!userConfirmed)
                        return;
                }

                var bankData = result.suggestions[0];

                // Заполняем свойства
                Counterparty.Bank = $"{bankData.data.name.payment} {bankData.data.address.value}";
                Counterparty.CorrAccount = bankData.data.correspondent_account;

                this.RaisePropertyChanged(nameof(Counterparty));
            }
            else
            {
                ShowWarning("Данные о банке не найдены.");
                return;
            }
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
