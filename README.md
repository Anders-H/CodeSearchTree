# CodeSearchTree
Simple Roslyn based code analysis library.

CodeSearchTree.dll uses Roslyn but does not require that your program references Roslyn.

## Search expressions

A search expression is a slash separated list of nodes. To get from a namespace to a method via a class, type:

`namespace/class/method`

Or:

`ns/cls/method`

If your namespace contains several classes, and your classes contains several methods, you can add an index. This will give you the third (index 2) method in the second (index 1) class.

`ns/cls[1]/method[2]`

You can also specify names instead of indexes. This will give you the method called MyFunction in the second class:

`ns/cls[1]/method[MyFunction]`

The node type can be unspecified. This will give you anything named MyFunction in the first class in the namespace.

`ns/cls/*[MyFunction]`

And this will deliver the first node with two parent nodes, regardless of name and type:

`*/*/*`

## C# Examples

Get class node from file:

```C#
var code_tree = CodeSearchTree.Node.CreateTreeFromFile(filename);
Var my_class = "ns/cls";
```

Search for file that contains node a class named MyClass:

```C#
var result = CodeSearchTree.FileSystem.CreateTreesFromFolder(foldername, "*/cls[MyClass]");
if (result.Count > 0)
   Console.WriteLine("Success!");
```
