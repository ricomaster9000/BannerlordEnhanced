namespace BannerlordEnhancedFramework.dialogues;

public enum AppliedDialogueLineRelation
{
    ManuallyLinked,
    LinkToRootStart,
    LinkToPreviousStart,
    LinkToNext,
    LinkToPrevious,
    LinkToPreviousEnd,
    LinkToCustom,
    LinkToNthChild
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