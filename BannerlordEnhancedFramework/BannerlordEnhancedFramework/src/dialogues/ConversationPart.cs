﻿using System;
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
        private int _dialogueTreeBranchDepth = 0;
        protected ConversationPart() {}

        public ConversationPart(ConversationPart conversationPart)
        {
            this._dialogueId = conversationPart._dialogueId;
            this._tokenId = conversationPart._tokenId;
            this._inGameName = conversationPart._inGameName;
            this._text = conversationPart._text;
            this._type = conversationPart._type;
            this._linkedCoreInputToken = conversationPart._linkedCoreInputToken;
            this._consequence = conversationPart._consequence;
            this._condition = conversationPart._condition;
            this._from = conversationPart._from;
            this._to = conversationPart._to;
            this._dialogueTreeBranchDepth = conversationPart._dialogueTreeBranchDepth;
        }

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
            this._consequence = consequence;
            return this;
        }

        public ConversationPart WithCondition(ConversationSentence.OnConditionDelegate condition)
        {
            this._condition = condition;
            return this;
        }

        private void testTrueFalse(bool boolvar)
        {
            
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
        
        public void SetTokenId(string tokenId)
        {
            this._tokenId = tokenId;
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

        public bool IsStartOfDialogueBranch()
        {
            return this._type.Equals(ConversationSentenceType.DialogueTreeBranchStart);
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

        public ConversationSentence.OnConsequenceDelegate Consequence()
        {
            return this._consequence;
        }

        public ConversationSentence.OnConditionDelegate Condition()
        {
            return this._condition;
        }

        public int GetDialogueTreeBranchDepth()
        {
            return this._dialogueTreeBranchDepth;
        }

        public void SetDialogueTreeBranchDepth(int dialogueTreeBranchDepth)
        {
            this._dialogueTreeBranchDepth = dialogueTreeBranchDepth;
        }
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