using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;

namespace BannerlordEnhancedFramework.dialogues;

public class DialogueBuilder
{
    private readonly string _dialogueId;
    private readonly List<CustomConversationSentence> _dialogueLinesToAdd = new();

    public DialogueBuilder(string dialogueId)
    {
        _dialogueId = dialogueId;
    }

    public DialogueBuilder AddCloseWindow(string tokenId, string text = "Close Window")
    {
        return WithDialogueLine(new CustomConversationSentence(
            tokenId,
            text, CustomConversationSentenceType.DialogueTreeRootEnd
        ).WithInGameName("Close Window"));
    }

    public DialogueBuilder AddEnd(string tokenId, string text = "Finish")
    {
        return WithDialogueLine(new CustomConversationSentence(
            tokenId,
            text, CustomConversationSentenceType.DialogueTreeRootEnd
        ).WithInGameName("Finish"));
    }

    public DialogueBuilder WithDialogueLine(
        CustomConversationSentence dialogueLine,
        AppliedDialogueLineRelation appliedDialogueLineRelation = AppliedDialogueLineRelation.ManuallyLinked,
        DialogueLineRelationParams dialogueLineRelationParams = null
    )
    {
        ApplyDialogueLineRelation(dialogueLine, appliedDialogueLineRelation, dialogueLineRelationParams);
        _dialogueLinesToAdd.Add(dialogueLine);
        return this;
    }

    public List<CustomConversationSentence> Build(CampaignGameStarter starter)
    {
        foreach (var conversationSentence in _dialogueLinesToAdd)
        {
            foreach (var toConversationSentence in conversationSentence.To)
            {
                if (toConversationSentence.IsStartOfDialogueTreeOrBranch())
                {
                    starter.AddPlayerLine(_dialogueId, conversationSentence.Id, toConversationSentence.Id,
                        "Go Back To '"+toConversationSentence.InGameName+"'", toConversationSentence.Condition,
                        toConversationSentence.Consequence);
                } else {
                    starter.AddPlayerLine(_dialogueId, conversationSentence.Id, toConversationSentence.Id,
                        toConversationSentence.Text, toConversationSentence.Condition,
                        toConversationSentence.Consequence);
                }
            }
        }
        return _dialogueLinesToAdd;
    }

    private void ApplyDialogueLineRelation(
        CustomConversationSentence customConversationSentence,
        AppliedDialogueLineRelation appliedDialogueLineRelation,
        DialogueLineRelationParams dialogueLineRelationParams)
    {
        switch (appliedDialogueLineRelation)
        {
            case AppliedDialogueLineRelation.LinkToPrevious:
                var previous = _dialogueLinesToAdd.Last();
                previous.To.Add(customConversationSentence);
                customConversationSentence.From = new List<CustomConversationSentence> { previous };
                break;
            case AppliedDialogueLineRelation.LinkToPreviousStart:
                var start = _dialogueLinesToAdd.FindLast(val => val.IsStartOfDialogueTreeOrBranch());
                start.From.Add(customConversationSentence);
                customConversationSentence.To.Add(start);
                break;
            case AppliedDialogueLineRelation.LinkToPreviousEnd:
                var end = _dialogueLinesToAdd.FindLast(val => val.IsEndOfDialogueTreeOrBranch());
                end.From.Add(customConversationSentence);
                customConversationSentence.To.Add(end);
                break;
        }
    }
}