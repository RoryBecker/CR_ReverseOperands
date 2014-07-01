CR_ReverseOperands
==================

**Purpose**

This plugin provider a new CodeProvider 'Reverse Operands'

It work on expressions like 

	100 * 5

or 

	"World" + "Hello"

It is designed to reverse the order of any 2 operands

**Usage**

Select the entire expression...

	<Operand1> <Operator> <Operand2>

Choose 'Reverse Operands' from the CodeRush SmartTag menu.

CodeRush will alter your highlighted expression so that the operands are reversed.

In our 2 previous examples, the results would be...

	5 * 100

and

	"Hello" + "World"
