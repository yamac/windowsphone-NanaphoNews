using System.Collections;
using System.Collections.ObjectModel;

namespace Helpers
{
    public partial class ObjectCollection : Collection<object>
    {
        public ObjectCollection()
        {
        }

        public ObjectCollection(IEnumerable collection)
        {
            foreach (object theObject in collection)
            {
                Add(theObject);
            }
        }
    }
}
