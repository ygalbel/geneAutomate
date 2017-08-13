// Differentiation towards the first heart field
#ExperimentFHF[0] |= $InitialFHF;
#ExperimentFHF[0] |= $JustBmp2I;
#ExperimentFHF[1] |= $Transita;
#ExperimentFHF[1] |= $JustBmp2IAndBmp2II;
#ExperimentFHF[2] |= $Transitb;
#ExperimentFHF[2] |= $JustBmp2IAndBmp2II;
#ExperimentFHF[3] |= $JustBmp2IAndBmp2II; 
#ExperimentFHF[3] |= $FinalFHF and fixpoint(#ExperimentFHF[3]);

// Differentiation towards the second heart field
#ExperimentSHF[0] |= $InitialSHF;
#ExperimentSHF[0] |= $JustBmp2IAndECanWntI;
#ExperimentSHF[1] |= $Transita;
#ExperimentSHF[1] |= $AllSignals;
#ExperimentSHF[2] |= $Transitb;
#ExperimentSHF[2] |= $AllSignals;
#ExperimentSHF[3] |= $AllSignals;
#ExperimentSHF[3] |= $FinalSHF and fixpoint(#ExperimentSHF[3]);

// Gene expression patterns from Herrmann et al

$InitialFHF :=
{
 eBmp2I = 1 and 
 eBmp2II = 0 and 
 ecanWntI = 0 and
 ecanWntII = 0 and
 canWnt = 1 and
 Foxc12 = 0 and 
 GATAs = 0 and
 Isl1 = 0 and 
 Mesp1 = 0 and 
 Nkx25 = 0 and
 Tbx1 = 0 and 
 Tbx5 = 0
};

$FinalFHF :=
{
 Bmp2   = 1 and
 Fgf8   = 0 and 
 Foxc12 = 0 and 
 GATAs = 1 and
 Isl1 = 0 and 
 Mesp1 = 0 and 
 Nkx25 = 1 and
 Tbx1 = 0 and 
 Tbx5 = 1 
};

$InitialSHF :=
{
 canWnt = 1 and
 Foxc12 = 0 and 
 GATAs = 0 and
 Isl1 = 0 and 
 Mesp1 = 0 and 
 Nkx25 = 0 and
 Tbx1 = 0 and 
 Tbx5 = 0 
};

$FinalSHF :=
{
 canWnt = 1 and 
 Fgf8   = 1 and 
 Foxc12 = 1 and 
 GATAs = 1 and
 Isl1 = 1 and 
 Mesp1 = 0 and 
 Nkx25 = 1 and
 Tbx1 = 1 and 
 Tbx5 = 0 
};

$Transita :=
{ 
 Foxc12 = 0 and 
 Mesp1 = 1 and 
 Nkx25 = 0 and
 Tbx1 = 0 and 
 Tbx5 = 0 
};

$Transitb :=
{
 canWnt = 0 and   
 GATAs = 1 and
 Isl1 = 1 and 
 Tbx5 = 1 
};

$AllSignals :=
{
 ecanWntI = 1 and
 ecanWntII = 1 and
 eBmp2I = 1 and 
 eBmp2II = 1 
};

$JustBmp2I :=
{
 ecanWntI = 0 and
 ecanWntII = 0 and
 eBmp2I = 1 and 
 eBmp2II = 0 
};

$JustBmp2IAndECanWntI :=
{
 ecanWntI = 1 and
 ecanWntII = 0 and
 eBmp2I = 1 and 
 eBmp2II = 0 
};

$JustBmp2IAndBmp2II :=
{
 ecanWntI = 0 and
 ecanWntII = 0 and
 eBmp2I = 1 and 
 eBmp2II = 1 
};