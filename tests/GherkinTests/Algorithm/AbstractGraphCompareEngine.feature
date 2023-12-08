Feature: AbstractGraphCompareEngine

Background:
Given an initial abstract state n11 in graph 1 with the following data
    | Name    | Value    |
    | stateId | abc12345 |
And another abstract state n12 in graph 1 with the following data
    | Name    | Value   |
    | stateId | abc5678 |
And an egde e12 in graph 1 to connect vertices n11 and n12 with the following data
    | Name     | Value   |
    | actionId | zxy1234 |

Given an initial abstract state n21 in graph 2 with the following data
    | Name    | Value    |
    | stateId | xyz12345 |
And another abstract state n22 in graph 2 with the following data
    | Name    | Value   |
    | stateId | xyz5678 |
And an egde e22 in graph 2 to connect vertices n21 and n22 with the following data
    | Name     | Value    |
    | actionId | abc12345 |

Given an initial abstract state n31 in graph 3 with the following data
    | Name    | Value    |
    | stateId | abc12345 |
And another abstract state n32 in graph 3 with the following data
    | Name    | Value   |
    | stateId | 1234567 |
And an egde e32 in graph 3 to connect vertices n31 and n32 with the following data
    | Name     | Value    |
    | actionId | abc12345 |

Given an initial abstract state n41 in graph 4 with the following data
    | Name    | Value    |
    | stateId | abc12345 |
And another abstract state n42 in graph 4 with the following data
    | Name    | Value   |
    | stateId | 1234567 |
And an egde e42 in graph 4 to connect vertices n41 and n42 with the following data
    | Name     | Value    |
    | actionId | abc12345 |
And another abstract state n43 in graph 4 with the following data
    | Name    | Value   |
    | stateId | 9876543 |
And an egde e43 in graph 4 to connect vertices n41 and n43 with the following data
    | Name     | Value    |
    | actionId | poiu1233 | 

Given an initial abstract state n51 in graph 5 with the following data
    | Name    | Value    |
    | stateId | initG5   |
And another abstract state n52 in graph 5 with the following data
    | Name    | Value   |
    | stateId | startG5 |
And another abstract state n53 in graph 5 with the following data
    | Name    | Value |
    | stateId | v1G5  |
And another abstract state n54 in graph 5 with the following data
    | Name    | Value   |
    | stateId | v2G5    |
And an egde e51 in graph 5 to connect vertices n51 and n52 with the following data
    | Name     | Value     |
    | actionId | gotostart |
And an egde e52 in graph 5 to connect vertices n52 and n51 with the following data
    | Name     | Value     |
    | actionId | goback    |
And an egde e53 in graph 5 to connect vertices n52 and n53 with the following data
    | Name     | Value     |
    | actionId | gov1      |
And an egde e54 in graph 5 to connect vertices n53 and n52 with the following data
    | Name     | Value     |
    | actionId | v1G5_tomain    |
And an egde e55 in graph 5 to connect vertices n52 and n54 with the following data
    | Name     | Value     |
    | actionId | gov2      |
And an egde e56 in graph 5 to connect vertices n54 and n52 with the following data
    | Name     | Value     |
    | actionId | v2G5_tomain    |

Given an initial abstract state n61 in graph 6 with the following data
    | Name    | Value    |
    | stateId | initG6   |
And another abstract state n62 in graph 6 with the following data
    | Name    | Value   |
    | stateId | startG6 |
And another abstract state n63 in graph 6 with the following data
    | Name    | Value   |
    | stateId | v2G6    |
And another abstract state n64 in graph 6 with the following data
    | Name    | Value   |
    | stateId | v3G6    |
And an egde e61 in graph 6 to connect vertices n61 and n62 with the following data
    | Name     | Value     |
    | actionId | gotostart |
And an egde e62 in graph 6 to connect vertices n62 and n61 with the following data
    | Name     | Value     |
    | actionId | goback    |
And an egde e63 in graph 6 to connect vertices n62 and n63 with the following data
    | Name     | Value     |
    | actionId | gov2      |
And an egde e64 in graph 6 to connect vertices n63 and n62 with the following data
    | Name     | Value     |
    | actionId | v2G6_tomain    |
And an egde e65 in graph 6 to connect vertices n62 and n64 with the following data
    | Name     | Value     |
    | actionId | gov3      |
And an egde e66 in graph 6 to connect vertices n64 and n62 with the following data
    | Name     | Value     |
    | actionId | v3G6_tomain    |

Given an initial abstract state n71 in graph 7 with the following data
    | Name    | Value    |
    | stateId | initialOBS |
And another abstract state n72 in graph 7 with the following data
    | Name    | Value   |
    | stateId | fileMenuState |
And another abstract state n73 in graph 7 with the following data
    | Name    | Value   |
    | stateId | settingsPanelState |
And an egde e71 in graph 7 to connect vertices n71 and n72 with the following data
    | Name     | Value    |
    | actionId | initialOBS_clickFileAction |
And an egde e72 in graph 7 to connect vertices n72 and n73 with the following data
    | Name     | Value    |
    | actionId | fileMenuState_clickSettingsAction | 
And an egde e73 in graph 7 to connect vertices n73 and n71 with the following data
    | Name     | Value    |
    | actionId | settingsPanelState_closeAction | 

Given an initial abstract state n81 in graph 8 with the following data
    | Name    | Value    |
    | stateId | initialOBS |
And another abstract state n82 in graph 8 with the following data
    | Name    | Value   |
    | stateId | fileMenuChangedState |
And another abstract state n83 in graph 8 with the following data
    | Name    | Value   |
    | stateId | settingsPanelState |
And an egde e81 in graph 8 to connect vertices n81 and n82 with the following data
    | Name     | Value    |
    | actionId | initialOBS_clickFileAction |
And an egde e82 in graph 8 to connect vertices n82 and n83 with the following data
    | Name     | Value    |
    | actionId | fileMenuChangedState_clickSettingsAction | 
And an egde e83 in graph 8 to connect vertices n83 and n81 with the following data
    | Name     | Value    |
    | actionId | settingsPanelState_closeAction | 

Scenario: Initial states are marked as corresponding states
For this test we use two graph that do not have a similar state id 
Given graph 1 as the old graph
And graph 2 as the new graph
When the comparison between the new and old graph has run
And the comparison result is merged
Then the merge contains 3 abstract states and 2 abstract actions
And the initial abstract state has the following data
    | Name               | Value    |
    | CD_CorrespondingId | abc12345 |
    | CD_CO_stateId      | abc12345 |
    | CD_CN_stateId      | xyz12345 |

Scenario: The same action id markes the target states as corresponding states
Given graph 2 as the old graph
And graph 3 as the new graph
When the comparison between the new and old graph has run
And the comparison result is merged
Then the merge contains 2 abstract states and 1 abstract action
And abstract state with stateId 1234567 has the following data
    | Name               | Value    |
    | CD_CorrespondingId | xyz5678  |
    | CD_CO_stateId      | xyz5678  |
    | CD_CN_stateId      | 1234567  |
And abstract state with stateId xyz5678 is not included in the merge graph

Scenario: A new state is merged as such
In graph 4 a new start (9876543 - n43) has been added
Given graph 3 as the old graph
And graph 4 as the new graph
When the comparison between the new and old graph has run
And the comparison result is merged
Then the merge contains 3 abstract states and 2 abstract actions
And abstract state with stateId 9876543 has the following class
    | ClassName     |
    | New           |
    | AbstractState |
    | NewVersion    |

Scenario: A removed state is merged as such
In graph 4 the state (9876543 - n43) has been added, so taking graph 3 al new graph
this will generate the removal of mentioned state
Given graph 4 as the old graph
And graph 3 as the new graph
When the comparison between the new and old graph has run
And the comparison result is merged
Then the merge contains 3 abstract states and 2 abstract actions
And abstract state with stateId 9876543 has the following class
    | ClassName     |
    | Removed       |
    | AbstractState |
    | OldVersion    |

Scenario: Two complex graphs 
Given graph 5 as the old graph
And graph 6 as the new graph
When the comparison between the new and old graph has run
And the comparison result is merged
Then the merge contains 5 abstract states and 9 abstract actions
And abstract state with stateId initG6 has the following class
    | ClassName     |
    | isInitial     |
    | Match         |
    | NewVersion    |
    | OldVersion    |
And abstract state with stateId startG6 has the following class
    | ClassName     |
    | Match         |
    | NewVersion    |
    | OldVersion    |
    | ContainsChanges |
And abstract state with stateId v2G6 has the following class
    | ClassName     |
    | Match         |
    | NewVersion    |
    | OldVersion    |
    | ContainsChanges |
And abstract state with stateId v1G5 has the following class
    | ClassName     |
    | Removed       |
    | OldVersion    |
And abstract state with stateId v3G6 has the following class
    | ClassName     |
    | New           |
    | NewVersion    |
And abstract state with stateId initG5 is not included in the merge graph
And abstract state with stateId startG5 is not included in the merge graph
And abstract state with stateId v2G5 is not included in the merge graph

Scenario: Two OBS graphs are doing a directional comparison
Given graph 7 as the old graph
And graph 8 as the new graph
When the comparison between the new and old graph has run
And the comparison result is merged
Then the merge contains 3 abstract states and 4 abstract actions
And abstract state with stateId initialOBS has the following class
    | ClassName       |
    | isInitial       |
    | Match           |
    | NewVersion      |
    | OldVersion      |
And abstract state with stateId fileMenuChangedState has the following class
    | ClassName       |
    | Match           |
    | ContainsChanges |
    | NewVersion      |
    | OldVersion      |
And abstract state with stateId settingsPanelState has the following class
    | ClassName       |
    | Match           |
    | NewVersion      |
    | OldVersion      |