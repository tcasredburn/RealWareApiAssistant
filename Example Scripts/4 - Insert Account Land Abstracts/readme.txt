Summary:
-------------------------------------
	These scripts will delete all Land Abstracts from an excel file (script A) and
	then add new Land Abstracts (script B).


Steps:
-------------------------------------
	1) Execute script A to delete all current abstracts on accounts in excel file.
	2) Execute script B to add all abstract data from excel file.


Note:
-------------------------------------
	It is recommended to use only one thread for this example since there
	are multiple abstracts being inserted on one account. If you want to use
	multiple threads, it would need to combine the abstracts into one API call.