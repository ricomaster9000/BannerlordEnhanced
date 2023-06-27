using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Conversation;

namespace BannerlordEnhancedFramework.dialogues
{

    public class ConversationPart
    {
        protected String _dialogueId;
        protected String _tokenId;
        protected String _inGameName;
        protected String _text;
        protected ConversationSentenceType _type = ConversationSentenceType.DialogueTreeBranchPart;
        protected InputToken _linkedCoreInputToken;

        private ConversationSentence.OnConsequenceDelegate _consequence;
        private ConversationSentence.OnConditionDelegate _condition;
        private readonly List<ConversationPart> _from = new List<ConversationPart>();
        private readonly List<ConversationPart> _to = new List<ConversationPart>();

        protected ConversationPart() {}

        public ConversationPart(string dialogueId)
        {
            this._dialogueId = dialogueId;
        }

        public ConversationPart(string dialogueId, string tokenId)
        {
            this._dialogueId = dialogueId;
            this._tokenId = tokenId;
        }

        public ConversationPart(string dialogueId, string tokenId, string text)
        {
            this._dialogueId = dialogueId;
            this._tokenId = tokenId;
            this._text = text;
        }

        public ConversationPart(string dialogueId, string tokenId, string text,
            ConversationSentenceType type)
        {
            this._dialogueId = dialogueId;
            this._tokenId = tokenId;
            this._text = text;
            this._type = type;
        }

        public ConversationPart(string dialogueId, string tokenId, string text,
            ConversationSentenceType type, InputToken linkedCoreInputToken)
        {
            this._dialogueId = dialogueId;
            this._tokenId = tokenId;
            this._text = text;
            this._type = type;
            this._linkedCoreInputToken = linkedCoreInputToken;
        }

        public ConversationPart WithInGameName(String inGameName)
        {
            this._inGameName = inGameName;
            return this;
        }

        public ConversationPart WithType(ConversationSentenceType conversationSentenceType)
        {
            _type = conversationSentenceType;
            return this;
        }

        public ConversationPart WithConsequence(ConversationSentence.OnConsequenceDelegate consequence)
        {
            Consequence = consequence;
            return this;
        }

        public ConversationPart WithCondition(ConversationSentence.OnConditionDelegate condition)
        {
            Condition = condition;
            return this;
        }

        public ConversationPart WithLinkedTo(ConversationPart to)
        {
            To().Add(to);
            return this;
        }

        public ConversationPart WithLinkedTo(List<ConversationPart> to)
        {
            To().AddRange(to);
            return this;
        }

        public string DialogueId()
        {
            return this._dialogueId;
        }

        public string TokenId()
        {
            return this._tokenId;
        }

        public InputToken LinkedCoreInputToken()
        {
            return this._linkedCoreInputToken;
        }

        public string InGameName()
        {
            if (this._inGameName == null)
            {
                this._inGameName = this._text;
            }
            return this._inGameName;
        }

        public ConversationSentenceType Type()
        {
            return this._type;
        }

        public string Text
        {
            get => _text;
            set => _text = value;
        }

        public List<ConversationPart> From()
        {
            return this._from;
        }

        public List<ConversationPart> To()
        {
            return this._to;
        }

        public bool IsStartOfDialogueTreeOrBranch()
        {
            return this._type.Equals(ConversationSentenceType.DialogueTreeRootStart) ||
                   this._type.Equals(ConversationSentenceType.DialogueTreeBranchStart);
        }

        public bool IsEndOfDialogueTreeOrBranch()
        {
            return this._type.Equals(ConversationSentenceType.DialogueTreeRootEnd) ||
                   this._type.Equals(ConversationSentenceType.DialogueTreeBranchEnd);
        }

        public ConversationSentence.OnConsequenceDelegate Consequence { get; set; }

        public ConversationSentence.OnConditionDelegate Condition { get; set; }
    }

    public class SimpleConversationPart : ConversationPart
    {
        public SimpleConversationPart(string tokenId, string text)
        {
            this._dialogueId = tokenId;
            this._tokenId = tokenId;
            this._text = text;
        }
        
        public SimpleConversationPart(string tokenId, string text, ConversationSentenceType conversationSentenceType)
        {
            this._dialogueId = tokenId;
            this._tokenId = tokenId;
            this._text = text;
            this._type = conversationSentenceType;
        }
        
        public SimpleConversationPart(string tokenId, string text, ConversationSentenceType conversationSentenceType, InputToken linkedCoreInputToken)
        {
            this._dialogueId = tokenId;
            this._tokenId = tokenId;
            this._text = text;
            this._type = conversationSentenceType;
            this._linkedCoreInputToken = linkedCoreInputToken;
        }
    }

}