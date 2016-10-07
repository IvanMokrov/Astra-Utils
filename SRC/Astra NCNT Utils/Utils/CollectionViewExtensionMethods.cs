using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

namespace Astra_NICNT_Utils.Utils
{

    public class CollectionViewProperty
    {
        public List<SortDescription> Sorts = new List<SortDescription>();
        public int Position;
    }


    public static class CollectionViewExtensionMethods
    {

        /// <summary> Get states such as sorting and positioning </summary>
        public static CollectionViewProperty GetStates(this CollectionViewSource source)
        {
            CollectionViewProperty result = new CollectionViewProperty();
            foreach (SortDescription description in source.View.SortDescriptions)
            {
                result.Sorts.Add(description);
            }
            result.Position = source.View.CurrentPosition;
            return result;
        }


        /// <summary> Set states such as sorting and positioning </summary>
        public static void SetStates(this CollectionViewSource source, CollectionViewProperty state)
        {
            source.View.SortDescriptions.Clear();
            foreach (SortDescription description in state.Sorts)
            {
                source.View.SortDescriptions.Add(description);
            }
            if (state.Position > (source.View as ListCollectionView).Count - 1)
                state.Position = (source.View as ListCollectionView).Count - 1;

            bool x = source.View.MoveCurrentToPosition(state.Position);
        }

    }
}
