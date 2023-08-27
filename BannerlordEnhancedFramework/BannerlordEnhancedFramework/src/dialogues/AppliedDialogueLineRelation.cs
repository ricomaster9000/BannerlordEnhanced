namespace BannerlordEnhancedFramework.dialogues;

public enum AppliedDialogueLineRelation
{
    ManuallyLinked,
    //LinkToRootStart,
    LinkToParentBranch,
    //LinkToNext,
    LinkToPrevious,
    LinkToCurrentBranch
    //LinkToCustom,
    //LinkToNthChild
}

public class DialogueLineRelationParams
{
    public DialogueLineRelationParams(int indexParam)
    {
        IndexParam = indexParam;
    }

    public DialogueLineRelationParams(int indexParam, ConversationPart conversationPartParam)
    {
        IndexParam = indexParam;
        ConversationPartParam = conversationPartParam;
    }

    public int IndexParam { get; set; }

    public ConversationPart ConversationPartParam { get; set; }
}