namespace Mentalfull.Permissions;

public static class MentalfullPermissions
{
    public const string GroupName = "Mentalfull";

    public static class JournalEntry
    {
        public const string Default = GroupName + ".JournalEntry";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string View = Default + ".View";
    }

    public static class AiSuggestion
    {
        public const string Default = GroupName + ".AiSuggestion";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string View = Default + ".View";
    }

    



    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";
}
