using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;

namespace BannerlordEnhancedFramework.dialogues;

/// <summary>
/// Class <c>DialogueBuilder</c> is a helper class that wraps logic and handles the creation of dialogues in the game
/// (it becomes more relevant in very complex big dialogues).
/// </summary>
public class DialogueBuilder
{
    private readonly List<ConversationPart> _dialogueLinesToAdd = new();

    public DialogueBuilder() {}

    public DialogueBuilder AddCloseWindow(string tokenId, string text = "Close Window")
    {
        return WithDialogueLine(new SimpleConversationPart(
            tokenId,
            text, ConversationSentenceType.DialogueTreeRootEnd
        ).WithInGameName("Close Window"));
    }

    public DialogueBuilder AddEnd(string tokenId, string text = "Finish")
    {
        return WithDialogueLine(new SimpleConversationPart(
            tokenId,
            text, ConversationSentenceType.DialogueTreeRootEnd
        ).WithInGameName("Finish"));
    }

    public DialogueBuilder WithDialogueLine(
        ConversationPart dialogueLine,
        AppliedDialogueLineRelation appliedDialogueLineRelation = AppliedDialogueLineRelation.ManuallyLinked,
        DialogueLineRelationParams dialogueLineRelationParams = null
    )
    {
        ApplyDialogueLineRelation(dialogueLine, appliedDialogueLineRelation, dialogueLineRelationParams);
        _dialogueLinesToAdd.Add(dialogueLine);
        return this;
    }

    /// <summary>
    /// Method <c>Build</c> Constructs and ADDS the dialogues to the game
    /// (it becomes more relevant in very complex big dialogues).
    /// </summary>
    public List<ConversationPart> Build(CampaignGameStarter starter)
    {
        foreach (ConversationPart conversationSentence in _dialogueLinesToAdd)
        {
            if (conversationSentence.Type() == ConversationSentenceType.DialogueTreeRootStart) {
                starter.AddPlayerLine(
                    conversationSentence.DialogueId() + "start",
                    conversationSentence.LinkedCoreInputToken().TokenName(),
                    conversationSentence.TokenId(),
                    conversationSentence.Text,
                    conversationSentence.Condition,
                    conversationSentence.Consequence);
                
                starter.AddDialogLine(
                    conversationSentence.DialogueId(),
                    conversationSentence.DialogueId(),
                    conversationSentence.TokenId() + "group",
                    conversationSentence.Text,
                    conversationSentence.Condition,
                    conversationSentence.Consequence);
            }

            foreach (ConversationPart toConversationSentence in conversationSentence.To())
            {
                if (toConversationSentence.IsStartOfDialogueTreeOrBranch())
                {
                    starter.AddPlayerLine(conversationSentence.TokenId()+"group",
                        conversationSentence.TokenId()+"group",
                        toConversationSentence.TokenId() + "start",
                        toConversationSentence.Text, toConversationSentence.Condition,
                        toConversationSentence.Consequence);
                    starter.AddDialogLine(
                        toConversationSentence.TokenId() + "start",
                        toConversationSentence.TokenId() + "start",
                        toConversationSentence.TokenId() + "group",
                        toConversationSentence.Text,
                        toConversationSentence.Condition,
                        toConversationSentence.Consequence);
                    starter.AddPlayerLine(toConversationSentence.TokenId() + "group", toConversationSentence.TokenId() + "group",
                        conversationSentence.DialogueId(),
                        "Go Back To '" + conversationSentence.InGameName() + "'", toConversationSentence.Condition,
                        toConversationSentence.Consequence);
                } else {
                    /*starter.AddPlayerLine(toConversationSentence.DialogueId(), conversationSentence.TokenId(),
                        conversationSentence.TokenId(),
                        "Go Back To '" + conversationSentence.InGameName + "'", toConversationSentence.Condition,
                        toConversationSentence.Consequence);*/
                    starter.AddPlayerLine(conversationSentence.TokenId()+"group", conversationSentence.TokenId()+"group",
                        conversationSentence.TokenId()+"start",
                        toConversationSentence.Text, toConversationSentence.Condition,
                        toConversationSentence.Consequence);
                }
            }
        }
        return _dialogueLinesToAdd;
    }

    private void ApplyDialogueLineRelation(
        ConversationPart conversationPart,
        AppliedDialogueLineRelation appliedDialogueLineRelation,
        DialogueLineRelationParams dialogueLineRelationParams)
    {
        switch (appliedDialogueLineRelation)
        {
            case AppliedDialogueLineRelation.LinkToPrevious:
                var previous = _dialogueLinesToAdd.Last();
                previous.To().Add(conversationPart);
                conversationPart.From().Add(previous);
                break;
            case AppliedDialogueLineRelation.LinkToPreviousStart:
                var start = _dialogueLinesToAdd.FindLast(val => val.IsStartOfDialogueTreeOrBranch());
                start.To().Add(conversationPart);
                conversationPart.From().Add(start);
                break;
            case AppliedDialogueLineRelation.LinkToPreviousEnd:
                var end = _dialogueLinesToAdd.FindLast(val => val.IsEndOfDialogueTreeOrBranch());
                end.To().Add(conversationPart);
                conversationPart.From().Add(end);
                break;
        }
    }
}
