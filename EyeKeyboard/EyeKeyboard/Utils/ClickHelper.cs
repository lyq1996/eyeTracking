using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace EyeKeyboard.Utils
{
    public static class  ClickHelper
    {

        public static void PerformClick(this ButtonBase button)
        {
            var method = button.GetType().GetMethod("OnClick",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(button, null);
            }


        }
    }
}
