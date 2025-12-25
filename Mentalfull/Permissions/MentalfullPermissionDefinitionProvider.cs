using Mentalfull.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Mentalfull.Permissions;

public class MentalfullPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(MentalfullPermissions.GroupName);




        //Define your own permissions here. Example:
        //myGroup.AddPermission(MentalfullPermissions.MyPermission1, L("Permission:MyPermission1"));

        var journalEntriesPermission = myGroup.AddPermission(MentalfullPermissions.JournalEntry.Default, L("Permission:JournalEntries"));
        journalEntriesPermission.AddChild(MentalfullPermissions.JournalEntry.Create, L("Permission:Create"));
        journalEntriesPermission.AddChild(MentalfullPermissions.JournalEntry.Edit, L("Permission:Edit"));
        journalEntriesPermission.AddChild(MentalfullPermissions.JournalEntry.Delete, L("Permission:Delete"));
        journalEntriesPermission.AddChild(MentalfullPermissions.JournalEntry.View, L("Permission:View")); 


        var aiSuggestionsPermission = myGroup.AddPermission(MentalfullPermissions.AiSuggestion.Default, L("Permission:AiSuggestions"));
        aiSuggestionsPermission.AddChild(MentalfullPermissions.AiSuggestion.Create, L("Permission:Create"));
        aiSuggestionsPermission.AddChild(MentalfullPermissions.AiSuggestion.Edit, L("Permission:Edit"));
        aiSuggestionsPermission.AddChild(MentalfullPermissions.AiSuggestion.Delete, L("Permission:Delete"));
        aiSuggestionsPermission.AddChild(MentalfullPermissions.AiSuggestion.View, L("Permission:View"));

    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MentalfullResource>(name);
    }
}
