namespace LiveOakApp.Models.Data.Entities
{
    public class CategoryOptionSelection
    {
        public readonly string OptionUID;
        public readonly string CategoryUID;
        public readonly bool IsSelected;

        public CategoryOptionSelection(string categoryUID, string optionUID, bool isSelected = false)
        {
            CategoryUID = categoryUID;
            OptionUID = optionUID;
            IsSelected = isSelected;
        }

        public CategoryOptionSelection InvertedSelection()
        {
            return new CategoryOptionSelection(CategoryUID, OptionUID, !IsSelected);
        }

        public override bool Equals(object obj)
        {
            var other = obj as CategoryOptionSelection;
            if (other == null) return false;
            if (!Equals(OptionUID, other.OptionUID)) return false;
            if (!Equals(CategoryUID, other.CategoryUID)) return false;
            if (!Equals(IsSelected, other.IsSelected)) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return OptionUID.GetHashCode() ^
                CategoryUID.GetHashCode() ^
                IsSelected.GetHashCode();
        }
    }
}
