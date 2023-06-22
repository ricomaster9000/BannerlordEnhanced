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

    public DialogueLineRelationParams(int indexParam, CustomConversationSentence conversationSentenceParam)
    {
        IndexParam = indexParam;
        ConversationSentenceParam = conversationSentenceParam;
    }

    public int IndexParam { get; set; }

    public CustomConversationSentence ConversationSentenceParam { get; set; }
}