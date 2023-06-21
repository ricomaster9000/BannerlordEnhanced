using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Conversation;

namespace BannerlordEnhancedFramework.dialogues;

public class CustomConversationSentence
{
    private String _tokenId;
    private String _inGameName;
    private String _text;
    private ConversationSentence.OnConsequenceDelegate _consequence;
    private ConversationSentence.OnConditionDelegate _condition;
    private List<CustomConversationSentence> _from;
    private List<CustomConversationSentence> _to;
    private CustomConversationSentenceType _type = CustomConversationSentenceType.DialogueTreeBranchPart;

    public CustomConversationSentence(string tokenId)
    {
        _tokenId = tokenId;
    }

    public CustomConversationSentence(string tokenId, string text)
    {
        _tokenId = tokenId;
        _text = text;
    }
    
    public CustomConversationSentence(string tokenId, string text, CustomConversationSentenceType type)
    {
        _tokenId = tokenId;
        _text = text;
        _type = type;
    }

    public CustomConversationSentence(string tokenId, string text, CustomConversationSentenceType type, List<CustomConversationSentence> from)
    {
        _tokenId = tokenId;
        _text = text;
        _type = type;
        From = from;
    }

    public CustomConversationSentence(string tokenId, string text, CustomConversationSentenceType type, List<CustomConversationSentence> from, List<CustomConversationSentence> to)
    {
        _tokenId = tokenId;
        _text = text;
        _type = type;
        From = from;
        To = to;
    }
    
    public CustomConversationSentence WithInGameName(String inGameName)
    {
        this._inGameName = inGameName;
        return this;
    }
    
    public CustomConversationSentence WithType(CustomConversationSentenceType customConversationSentenceType)
    {
        _type = customConversationSentenceType;
        return this;
    }

    public CustomConversationSentence WithConsequence(ConversationSentence.OnConsequenceDelegate consequence)
    {
        Consequence = consequence;
        return this;
    }

    public CustomConversationSentence WithCondition(ConversationSentence.OnConditionDelegate condition)
    {
        Condition = condition;
        return this;
    }

    public CustomConversationSentence WithLinkedTo(CustomConversationSentence to)
    {
        To.Add(to);
        return this;
    }

    public CustomConversationSentence WithLinkedTo(List<CustomConversationSentence> to)
    {
        To.AddRange(to);
        return this;
    }

    public string Id { get; set; }

    public string InGameName
    {
        get => _inGameName;
        set => _inGameName = value;
    }

    public CustomConversationSentenceType Type
    {
        get => _type;
        set => _type = value;
    }

    public string Text
    {
        get => _text;
        set => _text = value;
    }

    public List<CustomConversationSentence> From { get; set; }

    public List<CustomConversationSentence> To { get; set; }

    public bool IsStartOfDialogueTreeOrBranch()
    {
        return this._type.Equals(CustomConversationSentenceType.DialogueTreeRootStart) ||
               this._type.Equals(CustomConversationSentenceType.DialogueTreeBranchStart);
    }

    public bool IsEndOfDialogueTreeOrBranch()
    {
        return this._type.Equals(CustomConversationSentenceType.DialogueTreeRootEnd) ||
               this._type.Equals(CustomConversationSentenceType.DialogueTreeBranchEnd);
    }

    public ConversationSentence.OnConsequenceDelegate Consequence { get; set; }

    public ConversationSentence.OnConditionDelegate Condition { get; set; }
}