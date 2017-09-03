// Observation predicates
$Conditions1 := { S1 = 0 and S2 = 1};
$Conditions2 := { S1 = 0 and S2 = 1};
$Expression1 := {A = 1 and B = 1 and C = 1};

$Expression1_18 := {A = 0 and B = 1 and C = 1};
$Expression3 := {A = 1 and B = 1 and C = 1};
$Expression2_18 := {A = 0 and B = 1 and C = 1};


// Observations
#Experiment1[0] |= $Conditions1 and
#Experiment1[0] |= $Expression1 and
#Experiment1[18] |= $Expression1_18 and

#Experiment2[0] |= $Conditions2 and
#Experiment2[0] |= $Expression3 and
#Experiment2[18] |= $Expression2_18 and

#Experiment3[0] |= $Conditions1 and
#Experiment3[0] |= $Expression1 and
#Experiment3[18] |= $Expression1_18 and

#Experiment4[0] |= $Conditions2 and
#Experiment4[0] |= $Expression3 and
#Experiment4[18] |= $Expression2_18 and

#Experiment5[0] |= $Conditions1 and
#Experiment5[0] |= $Expression1 and
#Experiment5[18] |= $Expression1_18 and

#Experiment6[0] |= $Conditions2 and
#Experiment6[0] |= $Expression3 and
#Experiment6[18] |= $Expression2_18 and