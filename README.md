re# CodeSearchTree
Simple Roslyn based code analysis library.

CodeSearchTree.dll uses Roslyn but does not require that your program references Roslyn.

##Install version 1.0.4 from NuGet:

`Install-Package CodeSearchTree`

Download test client: [http://winsoft.se/files/CodeSearchTreeClient.zip](http://winsoft.se/files/CodeSearchTreeClient.zip)

## Search expressions

A search expression is a slash separated list of nodes. To get from a namespace to a method via a class, type:

`namespace/class/method`

Or:

`ns/cls/method`

If your namespace contains several classes, and your classes contains several methods, you can add an index. This will give you the third (index 2) method in the second (index 1) class.

`ns/cls[1]/method[2]`

You can also specify names instead of indexes. This will give you the method called `MyFunction` in the second class:

`ns/cls[1]/method[MyFunction]`

The node type can be unspecified. This will give you anything named `MyFunction` in the first class in the namespace.

`ns/cls/*[MyFunction]`

Read the value of a constant called `MyConstant` from file root:

`ns/cls/field[MyConstant]/vardeclaration/vardeclarator/equalsvalue/literal`

And this will deliver the first node with two parent nodes, regardless of name and type:

`*/*/*`

*Attribute* filter:

`ns/cls/method[@MyAttribute]`

*Return type* filter:

`ns/cls/property[#string]`

## C# Examples

Get class node from file:

```C#
var code_tree = CodeSearchTree.Node.CreateTreeFromFile(filename);
var my_class = code_tree.GetChild("ns/cls");
```

Acquire a child node, regardless of type, named `Main` and display its source.

```C#
var main = my_class.GetChild("*[Main]");
Console.WriteLine(main.Source);
```

Search for file that contains node a class named `MyClass`:

```C#
var result = CodeSearchTree.FileSystem.CreateTreesFromFolder(foldername, "*/cls[MyClass]");
if (result.Count > 0)
   Console.WriteLine("Success!");
```

Find the first test method in a file:

```C#
var code_tree = CodeSearchTree.Node.CreateTreeFromFile(filename);
var my_testmethod = code_tree.DeepSearch("method[@TestMethod]").FirstOrDefault();
Console.WriteLine(my_testmethod.Source);
```

Given that `my_class` references to a class, find property or method in a class that returns an `int`:

```C#
var x = my_class.GetChild("*[#int]");
```

Load a class file and get a reference to its constructor using the stronly typed API of CodeSearchTree.

```C#
//Load the C# class file.
var tree = CodeSearchTree.Node.CreateTreeFromFile(@"MyClass.cs");

//Get the first expression in the constructor.
var exp = tree.Ns.Cls.Constructor.SearchResult;
```

##Use cases

- [Read name and value from constants](http://www.winsoft.se/2015/12/codesearchtree-use-case-read-constant-values/)
- [List all invocations in constructor of a class](http://www.winsoft.se/2016/02/list-invocations-in-constructor/)