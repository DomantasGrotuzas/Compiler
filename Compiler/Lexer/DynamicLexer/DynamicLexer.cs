﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexer_Implementation.DynamicLexer.FSM;

namespace Lexer_Implementation.DynamicLexer
{
    public class DynamicLexer
    {
        private readonly StateMachine _stateMachine;
        public DynamicLexer(string pathToBNF)
        {
            var reader = new BNFReader(pathToBNF);
            var (rules, helpers) = reader.GetRules();

            LinkRules(rules, helpers);

            _stateMachine = new StateMachineBuilder().Build(rules, helpers);
        }

        public IEnumerable<Lexeme> GetLexemes(string code)
        {
            int globalOffset = 0;
            int iterationOffset = 0;
            for (; globalOffset + iterationOffset < code.Length; iterationOffset++)
            {
                if (!_stateMachine.Advance(code[globalOffset + iterationOffset]) || globalOffset + iterationOffset == code.Length - 1)
                {
                    if(_stateMachine.LastFinalState == null)
                        throw new ArgumentException($"Unrecognized sequence: {_stateMachine.Path}");
                    var lastState = _stateMachine.LastFinalState.Value;

                    globalOffset += lastState.value.Length;
                    iterationOffset = -1;

                    yield return new Lexeme
                    {
                        Value = lastState.value.Trim(),
                        Type = lastState.state.LexemeType
                    };
                    _stateMachine.Reset();
                }
            }
        }

        private void LinkRules(List<BNFRule> rules, List<BNFRule> helpers)
        {
            foreach (var rootRule in rules)
            {
                foreach (var rootRuleAlternative in rootRule.Alternatives)
                {
                    for (var i = 0; i < rootRuleAlternative.Count; i++)
                    {
                        if (!rootRuleAlternative[i].IsTerminal)
                        {
                            rootRuleAlternative[i] = GetRule(rootRuleAlternative[i].Name, rules.Union(helpers));
                        }
                    }
                }
            }
        }

        private BNFRule GetRule(string ruleName, IEnumerable<BNFRule> rules)
        {
            return rules.Single(r => r.Name == ruleName);
        }
    }
}