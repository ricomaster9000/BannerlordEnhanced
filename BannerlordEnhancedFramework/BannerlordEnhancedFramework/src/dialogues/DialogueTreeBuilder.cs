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
public class DialogueTreeBuilder
{
    // MAX CONVERSATION DEPTH OF 5 supported
    private readonly Dictionary<int, List<ConversationPart>>_dialogueLinesToAdd = new Dictionary<int, List<ConversationPart>>();
    private ConversationPart _currentTreeBranchStart = null;
    private int _currentTreeBranchDepthIndex = 0;

    public DialogueTreeBuilder() {}

    public DialogueTreeBuilder WithTrueFalseConversationToggle(
        ConversationPart conversationPart,
        AppliedDialogueLineRelation appliedDialogueLineRelation = AppliedDialogueLineRelation.AutomaticallyLinked,
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

    public DialogueTreeBuilder WithTrueFalseConversationToggle(
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

    public DialogueTreeBuilder WithConversationPart(
        ConversationPart conversationPart,
        AppliedDialogueLineRelation appliedDialogueLineRelation = AppliedDialogueLineRelation.AutomaticallyLinked,
        DialogueLineRelationParams dialogueLineRelationParams = null
    ) {
        return WithConversationParts(
            new List<ConversationPart>{conversationPart},
            appliedDialogueLineRelation,
            dialogueLineRelationParams
        );
    }

    public DialogueTreeBuilder WithConversationParts(
        ConversationPart conversationPart,
        ConversationPart conversationPart2,
        AppliedDialogueLineRelation appliedDialogueLineRelation = AppliedDialogueLineRelation.AutomaticallyLinked,
        DialogueLineRelationParams dialogueLineRelationParams = null
    ) {
        return WithConversationParts(
            new List<ConversationPart>{conversationPart,conversationPart2},
            appliedDialogueLineRelation,
            dialogueLineRelationParams
        );
    }
    
    public DialogueTreeBuilder WithConversationParts(
        List<ConversationPart> conversationParts,
        AppliedDialogueLineRelation appliedDialogueLineRelation = AppliedDialogueLineRelation.AutomaticallyLinked,
        DialogueLineRelationParams dialogueLineRelationParams = null
    ) {
        addConversationPart(conversationParts,appliedDialogueLineRelation,dialogueLineRelationParams);
        return this;
    }

    public DialogueTreeBuilder MoveBackToParentBranch()
    {
        _currentTreeBranchStart = _dialogueLinesToAdd[--_currentTreeBranchDepthIndex].First();
        return this;
    }
    
    public DialogueTreeBuilder MoveBackToRoot()
    {
        _currentTreeBranchDepthIndex = 0;
        _currentTreeBranchStart = _dialogueLinesToAdd[_currentTreeBranchDepthIndex].First();
        return this;
    }

    /// <summary>
    /// Method <c>Build</c> Constructs and ADDS the dialogues to the game
    /// (it becomes more relevant in very complex big dialogues).
    /// </summary>
    public Dictionary<int,List<ConversationPart>> Build(CampaignGameStarter starter)
    {
        foreach(KeyValuePair<int, List<ConversationPart>> conversationLists in _dialogueLinesToAdd) {
            foreach (ConversationPart conversationSentence in conversationLists.Value)
            {
                if (conversationSentence.Type() == ConversationSentenceType.DialogueTreeRootStart)
                {
                    starter.AddPlayerLine(
                        conversationSentence.DialogueId() + "group",
                        conversationSentence.LinkedCoreInputToken().TokenName(),
                        conversationSentence.TokenId() + "start",
                        conversationSentence.Text,
                        conversationSentence.Condition(),
                        conversationSentence.Consequence()
                    );

                    starter.AddDialogLine(
                        conversationSentence.DialogueId() + "start",
                        conversationSentence.TokenId() + "start",
                        conversationSentence.TokenId() + "group",
                        conversationSentence.Text,
                        conversationSentence.Condition(),
                        conversationSentence.Consequence()
                    );
                }

                if (conversationSentence.IsStartOfDialogueTreeOrBranch())
                {
                    starter.AddPlayerLine(
                        conversationSentence.TokenId() + "group",
                        conversationSentence.TokenId() + "group",
                        CoreInputToken.End.CloseWindow.TokenName(),
                        "Exit",
                        null,
                        null,
                        10
                    );
                    starter.AddPlayerLine(
                        conversationSentence.TokenId() + "group",
                        conversationSentence.TokenId() + "group",
                        CoreInputToken.End.ToStart.TokenName(),
                        "Back To Start",
                        null,
                        null,
                        5
                    );
                }

                foreach (ConversationPart toConversationSentence in conversationSentence.To())
                {
                    if (toConversationSentence.IsStartOfDialogueTreeOrBranch())
                    {
                        starter.AddPlayerLine(
                            toConversationSentence.TokenId() + "group",
                            toConversationSentence.TokenId() + "group",
                            conversationSentence.DialogueId() + "start",
                            "Go Back To '" + conversationSentence.InGameName() + "'",
                            toConversationSentence.Condition(),
                            toConversationSentence.Consequence(),
                            25
                        );
                        starter.AddPlayerLine(
                            conversationSentence.TokenId() + "group",
                            conversationSentence.TokenId() + "group",
                            toConversationSentence.TokenId() + "start",
                            toConversationSentence.Text,
                            toConversationSentence.Condition(),
                            toConversationSentence.Consequence()
                        );
                        starter.AddDialogLine(
                            toConversationSentence.TokenId() + "start",
                            toConversationSentence.TokenId() + "start",
                            toConversationSentence.TokenId() + "group",
                            toConversationSentence.Text,
                            toConversationSentence.Condition(),
                            toConversationSentence.Consequence()
                        );
                    }
                    else
                    {
                        starter.AddPlayerLine(
                            conversationSentence.TokenId() + "group",
                            conversationSentence.TokenId() + "group",
                            conversationSentence.TokenId() + "start",
                            toConversationSentence.Text,
                            toConversationSentence.Condition(),
                            toConversationSentence.Consequence());
                    }
                }
            }
        }
        return _dialogueLinesToAdd;
    }

    private void addConversationPart(
        List<ConversationPart> conversationParts,
        AppliedDialogueLineRelation appliedDialogueLineRelation,
        DialogueLineRelationParams dialogueLineRelationParams
    ) {
        foreach(ConversationPart conversationPart in conversationParts)
        {
            ApplyDialogueLineRelation(conversationPart,appliedDialogueLineRelation,dialogueLineRelationParams);
            if (conversationPart.IsStartOfDialogueTreeOrBranch())
            {
                _currentTreeBranchStart = conversationPart;
                _dialogueLinesToAdd[++_currentTreeBranchDepthIndex] = new List<ConversationPart>(){conversationPart};
            }
        }
    }

    private void ApplyDialogueLineRelation(
        ConversationPart conversationPart,
        AppliedDialogueLineRelation appliedDialogueLineRelation,
        DialogueLineRelationParams dialogueLineRelationParams
    )
    {
        ConversationPart branchStart = null;
        switch (appliedDialogueLineRelation)
        {
            case AppliedDialogueLineRelation.LinkToPrevious:
                var previous = _dialogueLinesToAdd[_currentTreeBranchDepthIndex].Last();
                previous.To().Add(conversationPart);
                conversationPart.From().Add(previous);
                _dialogueLinesToAdd[_currentTreeBranchDepthIndex].Add(conversationPart);
                break;
            case AppliedDialogueLineRelation.LinkToCurrentBranch:
                branchStart = _dialogueLinesToAdd[_currentTreeBranchDepthIndex].First();
                branchStart.To().Add(conversationPart);
                conversationPart.From().Add(branchStart);
                _dialogueLinesToAdd[_currentTreeBranchDepthIndex].Add(conversationPart);
                break;
            case AppliedDialogueLineRelation.LinkToParentBranch:
                var start = _dialogueLinesToAdd[_currentTreeBranchDepthIndex-1].First();
                start.To().Add(conversationPart);
                conversationPart.From().Add(start);
                break;
            case AppliedDialogueLineRelation.AutomaticallyLinked:
                if (conversationPart.IsStartOfDialogueBranch())
                {
                    branchStart = _dialogueLinesToAdd[_currentTreeBranchDepthIndex].First();
                    branchStart.To().Add(conversationPart);
                    conversationPart.From().Add(branchStart);
                }
                break;
        }
    }
}
