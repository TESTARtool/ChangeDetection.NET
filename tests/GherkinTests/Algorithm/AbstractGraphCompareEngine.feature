Feature: AbstractGraphCompareEngine

Background:
Given an abstract state n1 in graph 1 with the following data
    | Name       | Value    |
    | identifier | abc12345 |
    | isInitial  | True     |
And another abstract state n2 in graph 1 with the following data
    | Name       | Value   |
    | identifier | abc5678 |
    | isInitial  | False   |
And an egde e2 in graph 2 to connect verteces n1 and n2 with the following data
    | Name       | Value    |
    | identifier | zxy1234 |

Given an abstract state n1 in graph 1 with the following data
    | Name       | Value    |
    | identifier | abc12345 |
    | isInitial  | True     |
And another abstract state n2 in graph 1 with the following data
    | Name       | Value    |
    | identifier | abc12345 |
    | isInitial  | False     |
And an egde e2 in graph 2 to connect verteces n1 and n2 with the following data
    | Name       | Value    |
    | identifier | abc12345 |


Scenario: Initial states are marked as corresponding states
Given graph 1 as the old graph
And graph 2 as the new graph
When the comparison between the new and old graph has run
And the comparison result is merged
Then merge is not null



#
#@tag1
#Scenario: [scenario name]
#	Given graph g1 with an abstract state n1 with properties
#    | Name       | Value    |
#    | identifier | abc12345 |
#    | IsInitial  | True     |
#    And graph g1 with an abstract state n1 with properties
#    | Name       | Value    |
#    | identifier | abc12345 |
#    And graph g1 with edge e1 between abstract states n1 and n2 with properties
#    | Name     | Value  |
#    | actionid | 123456 |
#	When the compare has run
#	Then [outcome]
