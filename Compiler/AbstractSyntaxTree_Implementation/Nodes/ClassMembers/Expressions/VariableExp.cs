﻿using System;
using System.Collections.Generic;
using System.Text;
using AbstractSyntaxTree_Implementation.CodeGeneration;
using AbstractSyntaxTree_Implementation.Nodes.ClassMembers.Statements;
using AbstractSyntaxTree_Implementation.ResolveNames;
using Common;
using Lexer_Contracts;
using Type = AbstractSyntaxTree_Implementation.Nodes.Types.Type;

namespace AbstractSyntaxTree_Implementation.Nodes.ClassMembers.Expressions
{
    public class VariableExp : Expression, ITokenNode
    {
        public Node Target { get; set; }
        public string TokenType { get; set; }
        public string Value { get; set; }
        public int Line { get; set; }

        public override void ResolveNames(Scope scope)
        {
            Target = scope.ResolveName(new Name(this, NameType.Variable));
        }

        public override Type CheckTypes()
        {
            Type = (Type)Target?.GetType().GetProperty("Type")?.GetMethod.Invoke(Target, null);
            return Type;
        }

        public override void GenerateCode(CodeWriter w)
        {
            switch (Target)
            {
                case LocalVariableDeclaration l:
                    w.Write(Instr.I_GET_L, l.StackSlot);
                    break;
                case Parameter p:
                    w.Write(Instr.I_GET_L, p.StackSlot);
                    break;
                case VariableDeclaration h:
                    w.Write(Instr.I_GET_H, h.HeapSlot);
                    break;
                default:
                    throw new Exception($"{nameof(VariableExp)}".UnexpectedError(Line));
            }
        }
    }
}
