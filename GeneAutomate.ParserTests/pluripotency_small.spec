
// Experiment Three from 2i plus LIF to LIF plus PD
// Experiment Twenty Three overexpression of Tfcp2l1 in 2i plus LIF
#ExperimentTwentyThree[0] |=  $2iPlusLifTfcp2l1Overexpression "Exp23 initial expression pattern";
#ExperimentTwentyThree[0] |=  $JustPdCultureConditions "Exp23 culture conditions";
#ExperimentTwentyThree[0] |=  $NoKnockDowns "Exp23 no knockdowns";
#ExperimentTwentyThree[0] |=  $Tfcp2l1GeneOverExpression "Exp23 Tfcp2l1 overexpression";
#ExperimentTwentyThree[18] |=  $FinalStateTfcp2l1Overexpression "Exp23 penultimate state";
#ExperimentTwentyThree[19] |=  $FinalStateTfcp2l1Overexpression "Exp 23 final state";

// Culture conditions 

$FinalStateTfcp2l1Overexpression:=
{
 Klf4=0
};

$JustPdCultureConditions :=
{
 LIF = 0 and
 CH = 0 and 
 PD = 1
};

// Knock downs and overexpressions

$NoKnockDowns :=
{
 KO(Oct4)=0 and
 KO(Sox2)=0 and
 KO(Esrrb)=0 and
 KO(Stat3)=0 and
 KO(Nanog)=0
};

$Tfcp2l1GeneOverExpression:=
{
 FE(Esrrb)=0 and
 FE(Tfcp2l1)=1 
};

$2iPlusLifTfcp2l1Overexpression:=
{
 MEKERK = 0 and
 Oct4=1 and
 Sox2=1 and
 Nanog=1 and
 Esrrb=1 and
 Klf2=1 and
 Tfcp2l1=1 and
 Klf4=1 and
 Gbx2=1 and
 Tbx3=1 and
 Tcf3=0 and
 Sall4=1 and
 Stat3=1
};
