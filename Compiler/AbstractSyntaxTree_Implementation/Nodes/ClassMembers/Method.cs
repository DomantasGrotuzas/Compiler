﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntaxTree_Implementation.ResolveNames;
using Type = AbstractSyntaxTree_Implementation.Nodes.Types.Type;

namespace AbstractSyntaxTree_Implementation.Nodes.ClassMembers
{
    public class Method : ClassMember
    {
        public Visibility Visibility { get; set; }
        public TokenNode Virtual_Override { get; set; }
        public Type ReturnType { get; set; }
        public TokenNode Name { get; set; }
        public List<Parameter> Parameters { get; set; }
        public Body Body { get; set; }

        public override void Print(NodePrinter p)
        {
            p.Print(nameof(Visibility), Visibility);
            p.Print(nameof(Virtual_Override), Virtual_Override);
            p.Print(nameof(ReturnType), ReturnType);
            p.Print(nameof(Name), Name);
            p.Print(nameof(Parameters), Parameters);
            p.Print(nameof(Body), Body);
        }

        public override void ResolveNames(Scope scope)
        {
            scope = new Scope(scope);
            ReturnType.ResolveNames(scope);
            Parameters?.ForEach(x => x.ResolveNames(scope));
            Body?.ResolveNames(scope);
        }

        public override void AddName(Scope scope)
        {
            scope.Add(new Name(Name, NameType.Method), this);
        }

        public override Type CheckTypes()
        {
            if (Virtual_Override != null)
            {
                var refType = FindAncestor<Class>().Extends;
                if (refType == null)
                {
                    Console.WriteLine($"Method {Name.Value}, cannot be marked with {Virtual_Override.Value} because this class doesn't extend any other class. Line {Name.Line}");
                }
                else
                {
                    while (refType != null)
                    {
                        var method = (Method)refType.Target?.Body?.Members.FirstOrDefault(x => x is Method m && m.Name.Value == Name.Value);
                        if (method == null)
                        {
                            refType = refType.Target?.Extends;
                        }
                        else
                        {
                            var expectedParamCount = method.Parameters?.Count ?? 0;
                            var actualParamCount = Parameters?.Count ?? 0;

                            if (expectedParamCount != actualParamCount)
                                Console.WriteLine($"Expected {expectedParamCount} parameters, got {actualParamCount}");

                            for (int i = 0; i < Math.Min(expectedParamCount, actualParamCount); i++)
                            {
                                method.Parameters?[i].Type.IsEqual(Parameters?[i].Type);
                            }

                            break;
                        }
                    }
                    if(refType == null)
                        Console.WriteLine($"Method {Name.Value} doesn't exist on base type");
                }
            }

            Body?.CheckTypes();

            return null;
        }
    }
}
