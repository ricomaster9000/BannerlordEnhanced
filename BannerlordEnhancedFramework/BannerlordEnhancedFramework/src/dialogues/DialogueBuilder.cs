using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BannerlordEnhancedFramework.utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.dialogues;

/// <summary>
/// Class <c>DialogueBuilder</c> is a helper class that wraps logic and handles the creation of dialogues in the game
/// (it becomes more relevant in very complex big dialogues).
/// </summary>
public class DialogueBuilder
{
    private readonly List<ConversationPart> _dialogueLinesToAdd = new List<ConversationPart>();

    public DialogueBuilder() {}

    public DialogueBuilder WithTrueFalseConversationToggle(
        ConversationPart conversationPart,
        AppliedDialogueLineRelation appliedDialogueLineRelation = AppliedDialogueLineRelation.ManuallyLinked,
        DialogueLineRelationParams dialogueLineRelationParams = null
    ) {
        if (conversationPart.Consequence != null && conversationPart.Consequence().Method.GetParameters().Length > 0) {
            throw new Exception("You can only pass in a consequence method with zero params, " +
                                "when this method is used it assumes you are using a toggle method " +
                                "rather than a method were you specify true or false as a parameter");
        }
        
        return WithTrueFalseConversationToggle(
            conversationPart,
            appliedDialogueLineRelation,
            dialogueLineRelationParams,
            conversationPart.Consequence()
        );
    }

    public DialogueBuilder WithTrueFalseConversationToggle(
        ConversationPart conversationPart,
        AppliedDialogueLineRelation appliedDialogueLineRelation,
        DialogueLineRelationParams dialogueLineRelationParams,
        ConversationSentence.OnConsequenceDelegate falseConsequence
    ) {
        ConversationPart conversationPartFalse = new ConversationPart(conversationPart)
            .WithCondition(() => !conversationPart.Condition().Invoke())
            .WithConsequence(falseConsequence);
        conversationPartFalse.SetTokenId(conversationPartFalse.TokenId()+"_false");
        conversationPartFalse.Text += " - Currently False";
        conversationPart.Text += " - Currently True";
        return WithConversationParts(
            conversationPart,
            conversationPartFalse,
            appliedDialogueLineRelation,
            dialogueLineRelationParams
        );
    }

    public DialogueBuilder WithConversationParts(
        ConversationPart conversationPart,
        ConversationPart conversationPart2,
        AppliedDialogueLineRelation appliedDialogueLineRelation = AppliedDialogueLineRelation.ManuallyLinked,
        DialogueLineRelationParams dialogueLineRelationParams = null
    ) {
        return WithConversationParts(
            new List<ConversationPart>{conversationPart,conversationPart2},
            appliedDialogueLineRelation,
            dialogueLineRelationParams
        );
    }
    
    public DialogueBuilder WithConversationParts(
        List<ConversationPart> conversationParts,
        AppliedDialogueLineRelation appliedDialogueLineRelation = AppliedDialogueLineRelation.ManuallyLinked,
        DialogueLineRelationParams dialogueLineRelationParams = null
    ) {
        ApplyDialogueLineRelations(conversationParts,appliedDialogueLineRelation,dialogueLineRelationParams);
        _dialogueLinesToAdd.AddRange(conversationParts);
        return this;
    }

    public DialogueBuilder WithConversationPart(
        ConversationPart conversationPart,
        AppliedDialogueLineRelation appliedDialogueLineRelation = AppliedDialogueLineRelation.ManuallyLinked,
        DialogueLineRelationParams dialogueLineRelationParams = null
    ) {
        ApplyDialogueLineRelation(conversationPart,appliedDialogueLineRelation,dialogueLineRelationParams);
        _dialogueLinesToAdd.Add(conversationPart);
        return this;
    }

    /// <summary>
    /// Method <c>Build</c> Constructs and ADDS the dialogues to the game
    /// (it becomes more relevant in very complex big dialogues).
    /// </summary>
    public List<ConversationPart> Build(CampaignGameStarter starter)
    {
        foreach (ConversationPart conversationSentence in _dialogueLinesToAdd) {
            if (conversationSentence.Type() == ConversationSentenceType.DialogueTreeRootStart) {
                starter.AddPlayerLine(
                    conversationSentence.DialogueId()+"group",
                    conversationSentence.LinkedCoreInputToken().TokenName(),
                    conversationSentence.TokenId()+"start",
                    conversationSentence.Text,
                    conversationSentence.Condition(),
                    conversationSentence.Consequence()
                );

                starter.AddDialogLine(
                    conversationSentence.DialogueId()+"start",
                    conversationSentence.TokenId()+"start",
                    conversationSentence.TokenId()+"group",
                    conversationSentence.Text,
                    conversationSentence.Condition(),
                    conversationSentence.Consequence()
                );
            }

            if (conversationSentence.IsStartOfDialogueTreeOrBranch()) {
                starter.AddPlayerLine(
                    conversationSentence.TokenId()+"group",
                    conversationSentence.TokenId()+"group",
                    CoreInputToken.End.CloseWindow.TokenName(),
                    "Exit",
                    null,
                    null, 
                    10
                );
                starter.AddPlayerLine(
                    conversationSentence.TokenId()+"group",
                    conversationSentence.TokenId()+"group",
                    CoreInputToken.End.ToStart.TokenName(),
                    "Back To Start",
                    null,
                    null,
                    5
                );
            }

            foreach (ConversationPart toConversationSentence in conversationSentence.To()) {
                if (toConversationSentence.IsStartOfDialogueTreeOrBranch()) {
                    starter.AddPlayerLine(
                        toConversationSentence.TokenId()+"group",
                        toConversationSentence.TokenId()+"group",
                        conversationSentence.DialogueId()+"start",
                        "Go Back To '"+conversationSentence.InGameName()+"'",
                        toConversationSentence.Condition(),
                        toConversationSentence.Consequence(),
                        25
                    );
                    starter.AddPlayerLine(
                        conversationSentence.TokenId()+"group",
                        conversationSentence.TokenId()+"group",
                        toConversationSentence.TokenId()+"start",
                        toConversationSentence.Text,
                        toConversationSentence.Condition(),
                        toConversationSentence.Consequence()
                    );
                    starter.AddDialogLine(
                        toConversationSentence.TokenId()+"start",
                        toConversationSentence.TokenId()+"start",
                        toConversationSentence.TokenId()+"group",
                        toConversationSentence.Text,
                        toConversationSentence.Condition(),
                        toConversationSentence.Consequence()
                    );
                } else {
                    starter.AddPlayerLine(
                        conversationSentence.TokenId()+"group",
                        conversationSentence.TokenId()+"group",
                        conversationSentence.TokenId()+"start",
                        toConversationSentence.Text,
                        toConversationSentence.Condition(),
                        toConversationSentence.Consequence());
                }
            }
        }
        return _dialogueLinesToAdd;
    }

    private void ApplyDialogueLineRelations(
        List<ConversationPart> conversationParts,
        AppliedDialogueLineRelation appliedDialogueLineRelation,
        DialogueLineRelationParams dialogueLineRelationParams
    ) {
        foreach(ConversationPart conversationPart in conversationParts)
        {
            ApplyDialogueLineRelation(conversationPart,appliedDialogueLineRelation,dialogueLineRelationParams);
        }
    }

    private void ApplyDialogueLineRelation(
        ConversationPart conversationPart,
        AppliedDialogueLineRelation appliedDialogueLineRelation,
        DialogueLineRelationParams dialogueLineRelationParams
    ) {
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
