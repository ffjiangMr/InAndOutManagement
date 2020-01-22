namespace InOutManagement.ExtensionMethon
{
    #region using directive

    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Media;

    #endregion
    public sealed class VisualHelper
    {
        public static DependencyObject GetChild(ref String name, ref Type type, ref DependencyObject parent)
        {
            DependencyObject result = null;
            for (Int32 flag = 0; flag < VisualTreeHelper.GetChildrenCount(parent); flag++)
            {
                if (result != null)
                {
                    break;
                }
                DependencyObject child = VisualTreeHelper.GetChild(parent, flag);
                if (child.GetType() == type)
                {
                    var property = type.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public);
                    if (property != null)
                    {
                        var propVaulue = (String)property.GetValue(child, null);
                        if (propVaulue == name)
                        {
                            result = child;
                            break;
                        }
                    }
                }                                
                result = GetChild(ref name, ref type, ref child);
            }
            return result;
        }
    }
}
