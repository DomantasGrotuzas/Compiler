﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Lexer_Implementation.DynamicLexer.FSM
{
    public class StateMachine
    {
        private readonly State _startState;
        private State _currentState;
        public string Path { get; private set; }

        public (State state, string value)? LastFinalState { get; private set; }
    
        public StateMachine(State startState)
        {
            _startState = startState;
            Reset();
        }

        //false jei nera tokio perejimo
        public bool Advance(char c)
        {
            var transition = _currentState.Transitions.SingleOrDefault(t => t.Conditions.Contains(c));
            if (transition == null)
                return false;

            _currentState = transition.To;

            Path += c;

            if (_currentState.IsFinal)
                LastFinalState = (state: _currentState, value: Path);

            return true;
        }
        public void Reset()
        {
            _currentState = _startState;
            Path = string.Empty;
            LastFinalState = null;
        }
    }
}
