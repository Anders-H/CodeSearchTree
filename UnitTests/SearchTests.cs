using CodeSearchTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class SearchTests
    {
        [TestMethod]
        public void TypedSearchInNodeList()
        {
            const string filename = @"C:\Jobb\TS-Compiler\Stordatorkod\Development\Development\Database\A5560.cs";
            var tree = Node.CreateTreeFromFile(filename);
            Assert.IsTrue(tree.UsingDirective.SearchResult.Source == "using System;", "Find using, no indexer.");
            Assert.IsTrue(tree.UsingDirective[0].Source == "using System;", "Find first using.");
            Assert.IsTrue(tree.UsingDirective[1].Source == "using System.Collections.Generic;", "Find second using.");
            Assert.IsTrue(tree.Ns[0].Source.StartsWith("namespace TS.GUNNAR.BusinessLogic.Database"), "Find namespace, indexer.");
            Assert.IsTrue( tree.Ns.SearchResult.Source.StartsWith("namespace TS.GUNNAR.BusinessLogic.Database {"), "Find namespace, no indexer.");
            Assert.IsTrue(tree.Ns[0].Cls[0].Source.StartsWith("public sealed class A5560"), "Find class, indexers.");
            Assert.IsTrue(tree.Ns.Cls.SearchResult.Source.StartsWith("public sealed class A5560"), "Find class, no indexers.");
            Assert.IsTrue(
                tree.Ns["Company.STA_PSPEC_AMT.BusinessLogic.Database"].Cls["A5560"].Source.StartsWith(
                    "public sealed class A5560"), "Find class by name.");
            var classNode = tree.GetChild("ns/cls");
            Assert.IsNotNull(classNode, "Failed to get class node.");
            var tableNameField = classNode.Field["TABLENAME"];
            Assert.IsNotNull(tableNameField, "Failed to use typed field with name.");
            var id =
                classNode.Field["TABLENAME"]?.GetChild("vardeclaration/vardeclarator/equalsvalue/literal")?.Source ?? "";
            Assert.IsTrue(id == "\"A5560\"", "Failed mixed chain.");
            var x = classNode.Field["TABLENAME"]?.VarDeclaration?.SearchResult?.Source ?? "";
            Assert.IsFalse(string.IsNullOrEmpty(x), "Failed: VarDeclaration.");
            x = classNode.Field["TABLENAME"]?.VarDeclaration?.VarDeclarator?.SearchResult?.Source ?? "";
            Assert.IsFalse(string.IsNullOrEmpty(x), "Failed: VarDeclarator.");
            x = classNode.Field["TABLENAME"]?.VarDeclaration?.VarDeclarator?.EqualsValue?.SearchResult?.Source ?? "";
            Assert.IsFalse(string.IsNullOrEmpty(x), "Failed: EqualsValue.");
            var tableName =
                classNode.Field["TABLENAME"]?.VarDeclaration?.VarDeclarator?.EqualsValue?.Literal?.SearchResult?.Source ??
                "";
            Assert.IsFalse(string.IsNullOrEmpty(tableName), "Failed to extract table name using typed api.");
        }

        [TestMethod]
        public void BugfixForTableEditorTest()
        {
            const string filename = @"C:\Jobb\TS-Compiler\Stordatorkod\Development\Development\Database\A5560.cs";
            var classNode = Node.CreateTreeFromFile(filename).Ns?.Cls?.SearchResult;
            Assert.IsNotNull(classNode, "Failed to load class.");
            //Find the calls that initializes the fields, read out encrypted, initial value and special field.
            var expressionInConstructor = classNode.Constructor?.Block?.Expression?.SearchResult;
            Assert.IsNotNull(classNode, "Failed locate expression.");
            var c = expressionInConstructor?.Assign?.Invocation?.MemberAccess?.SearchResult;
            Assert.IsNotNull(c, "Failed to locate CreateFieldAlpha.");
            c = c?.Id["CreateFieldAlpha"];

            var functionNameNode = expressionInConstructor.Assign?.Invocation?.SearchResult;
            functionNameNode = functionNameNode?.MemberAccess?.SearchResult;
            Assert.IsNotNull(functionNameNode, "Failed to locate name of invoked function.");
            functionNameNode = functionNameNode?.Id[1];
        }
    }
}
