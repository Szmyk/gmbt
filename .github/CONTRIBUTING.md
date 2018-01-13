# Commiting changes

There are several rules for naming commits messages:

* English language
* The name should begin with a capital letter
* The name should not be longer than 70 characters, the optimum amount is 50
* No dots at the end

# Coding

* Comments in English
* Each comment should contain one of these prefixes:
    * **TODO** - something to do, eg. functionality
    * **FIXME** - something to improve, mostly commented code that does not work
    * **CHGME** - something works but needs improvement
    * **HACK** - sometimes it is needed to describe the hack you used to get people to understand your intentions
    * **UNDONE** - something is unfinished and needs completion
    * **REFACTOR** - requires refactoring or improving coding standards, etc.
    * **IDEA** - an idea
    * **TEMP** - the code is temporary and will be replaced in the future

* Use [PascalCase](https://en.wikipedia.org/wiki/PascalCase) for class names, method names and member public variables

* Use [camelCase](https://en.wikipedia.org/wiki/Camel_case) for method arguments, local and member private variables

* Do not use [Hungarian notation](https://en.wikipedia.org/wiki/Hungarian_notation) or any other type identification in identifiers

* Declare all member variables at the top of a class, with static variables at the very top
