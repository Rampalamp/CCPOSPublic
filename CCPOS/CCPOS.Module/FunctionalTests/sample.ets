;#DropDB CCPOSEasyTest

#Application CCPOSWin
#Application CCPOSWeb

*FillForm
 User Name = Admin
*Action Log In

#IfDef CCPOSWin

*Action About...
 
*Action OK

#EndIf

#IfDef CCPOSWeb
*Action Navigation(My Details)
#EndIf
