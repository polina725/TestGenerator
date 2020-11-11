﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Generator
{
    public class Generator
    {
        private List<GeneratedFileInfo> list = new List<GeneratedFileInfo>();
        private string outputFolder;

        public Generator(string outputFolder)
        {
            this.outputFolder = outputFolder;
        }

        public List<GeneratedFileInfo> AnalyseFile(string sourceCode)
        {
            SyntaxNode root = CSharpSyntaxTree.ParseText(sourceCode).GetRoot();
            foreach (ClassDeclarationSyntax cl in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                ClassDeclarationSyntax testClass = CreateTestClass(cl.Identifier.ValueText);
                IEnumerable<SyntaxNode> methods = cl.DescendantNodes().OfType<MethodDeclarationSyntax>().Where(method => method.Modifiers.Any(SyntaxKind.PublicKeyword));
                foreach (SyntaxNode method in methods)
                {

                    testClass = testClass.AddMembers(CreateTestMethod((method as MethodDeclarationSyntax).Identifier.ValueText));
                }
                CompilationUnitSyntax unit = CompilationUnit().WithUsings(GetUsings()).AddMembers(NamespaceDeclaration(ParseName("tests")).AddMembers(testClass));
                list.Add(new GeneratedFileInfo($"{outputFolder}\\{cl.Identifier.ValueText}Tests.cs", unit.NormalizeWhitespace().ToFullString()));
            }
            return list;
            
        }

        private static SyntaxList<UsingDirectiveSyntax> GetUsings()
        {
            List<UsingDirectiveSyntax> defaultUsings = new List<UsingDirectiveSyntax>
            {
                UsingDirective(IdentifierName("System")),
                UsingDirective(QualifiedName(IdentifierName("System"), IdentifierName("Linq"))),
                UsingDirective(QualifiedName(QualifiedName(IdentifierName("System"), IdentifierName("Collections")), IdentifierName("Generic"))),
                UsingDirective(QualifiedName(IdentifierName("NuGet"), IdentifierName("Frameworks")))
            };
            return List(defaultUsings);
        }

        private ClassDeclarationSyntax CreateTestClass(string className)
        {
            AttributeSyntax attr = Attribute(ParseName("TestClass"));
            ClassDeclarationSyntax testClass = ClassDeclaration(className + "Test").AddModifiers(Token(SyntaxKind.PublicKeyword));
            File.WriteAllText($"{className}.cs", testClass.GetText().ToString());
            return testClass; 
        }

        private MethodDeclarationSyntax CreateTestMethod(string methodName)
        {
            AttributeSyntax attr = Attribute(ParseName("TestMethod"));
            MethodDeclarationSyntax testMethod = MethodDeclaration(ParseTypeName("void"), methodName + "Test").AddModifiers(Token(SyntaxKind.PublicKeyword)).
                                                                    AddBodyStatements(FormMethodBody()).AddAttributeLists(AttributeList().AddAttributes(attr));
            return testMethod;
        }


        private StatementSyntax[] FormMethodBody()
        {
            StatementSyntax[] body = { ParseStatement("Assert.Fail(\"autogenerated\");") };
            return body;
        }
    }
}