using System;
using Microsoft.AspNetCore.Components;
using NutritionWebClient.Services.InformationDialog;

namespace NutritionWebClient.Components.InformationDialog
{
    public partial class InformationDialog : ComponentBase, IDisposable
    {
        [Inject]
        public InformationDialogService _informationDialogService { get; set; }

        protected string Heading { get; set; }
        protected string Message { get; set; }
        protected bool IsVisible { get; set; }
        protected string BackgroundCssClass { get; set; }
        protected string IconCssClass { get; set; }

        protected override void OnInitialized()
        {
            _informationDialogService.OnShow += ShowInformationDialog;
            _informationDialogService.OnHide += HideInformationDialog;
            
            base.OnInitialized();
        }

        private void HideInformationDialog()
        {
            Console.WriteLine($"[HideInformationDialog] Dialog should hide.");
            IsVisible = false;
            InvokeAsync(StateHasChanged);
        }

        private void ShowInformationDialog(string message, DialogType type)
        {
            Console.WriteLine($"[ShowInformationDialog] Dialog should be visible.");
            CreateInformationDialog(type, message);
            IsVisible = true;
            StateHasChanged();
        }

        private void CreateInformationDialog(DialogType type, string message)
        {
            switch(type)
            {
                case DialogType.Success:
                    BackgroundCssClass = $"bg-success";
                    IconCssClass = "check";
                    Heading = "Success";
                    break;
                case DialogType.Error:
                    BackgroundCssClass = "bg-danger";
                    IconCssClass = "times";
                    Heading = "Error";
                    break;
            }

            Message = message;
        }

        public void Dispose()
        {
            _informationDialogService.OnShow -= ShowInformationDialog;
        }
    }
}