using System.Windows;
using System.Windows.Controls;

namespace Helpers
{
    public interface IResourceDataTemplate
    {
        string ResourceKey { get; }
    }

    public class ResourceDataTemplateSelector : ContentControl
    {
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            DataTemplate contentTemplate = null;
            if (newContent is IResourceDataTemplate && newContent != null)
            {
                var contentTypeName = (newContent as IResourceDataTemplate).ResourceKey;
                contentTemplate = this.Resources[contentTypeName] as DataTemplate;
            }
            this.ContentTemplate = contentTemplate;
        }
    }
}
