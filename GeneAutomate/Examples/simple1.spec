// Observation predicates
$Expression1 := {A = 1 and B = 0 and C = 1};
$Expression11 := {A = 1 and B = 1 and C = 0};
$Expression2 := {A = 1 and B = 1 and C = 0};
$Expression22 := {A = 1 and B = 0 and C = 1};

// Observations
#Experiment1[0] |= $Expression1
#Experiment1[3] |= $Expression11

#Experiment2[0] |= $Expression2
#Experiment2[3] |= $Expression22
