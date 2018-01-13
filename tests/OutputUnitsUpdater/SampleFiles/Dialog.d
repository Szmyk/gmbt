instance DIA_Npc_Test (C_INFO)
{
	npc			= Npc;
	nr			= 1;
	condition	= DIA_Npc_Test_Condition;
	information	= DIA_Npc_Test_Info;
	permanent	= true;
	description = "...";
};                       

func int DIA_Npc_Test_Condition()
{
	return true;
};

func void DIA_Npc_Test_Info()
{
	AI_Output (other, self, "DIA_Npc_Test_15_00");  //.
	AI_Output (self,other, "DIA_Npc_Test_01_01"); //..
	AI_Output (self, other,"DIA_Npc_Test_01_02"); //...
	AI_Output (other, self,"DIA_Npc_Test_15_03" ); //.
	AI_Output (self,other,"DIA_Npc_Test_01_04"); //..
	AI_Output (self,other, "DIA_Npc_Test_01_05" ); //...
};
