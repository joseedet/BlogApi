using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAdmin.Services;

public class ToastService
{
    public event Action<string>? OnShow;

    public void ShowToast(string mensaje)
    {
        OnShow?.Invoke(mensaje);
    }
}
