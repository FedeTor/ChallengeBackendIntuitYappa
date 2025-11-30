‚
§C:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Application\Clientes\Queries\SearchClientesByName\SearchClientesByNameQueryHandler.cs
	namespace 	
Clientes
 
. 
Application 
. 
Clientes '
.' (
Queries( /
./ 0 
SearchClientesByName0 D
;D E
public 
class ,
 SearchClientesByNameQueryHandler -
:. /
IRequestHandler0 ?
<? @%
SearchClientesByNameQuery@ Y
,Y Z
IReadOnlyList[ h
<h i
Clientei p
>p q
>q r
{		 
private

 
readonly

 "
IClienteReadRepository

 +"
_clienteReadRepository

, B
;

B C
private 
readonly 
ILogger 
< ,
 SearchClientesByNameQueryHandler =
>= >
_logger? F
;F G
public 
,
 SearchClientesByNameQueryHandler +
(+ ,"
IClienteReadRepository, B!
clienteReadRepositoryC X
,X Y
ILoggerZ a
<a b-
 SearchClientesByNameQueryHandler	b Ç
>
Ç É
logger
Ñ ä
)
ä ã
{ "
_clienteReadRepository 
=  !
clienteReadRepository! 6
;6 7
_logger 
= 
logger 
; 
} 
public 

async 
Task 
< 
IReadOnlyList #
<# $
Cliente$ +
>+ ,
>, -
Handle. 4
(4 5%
SearchClientesByNameQuery5 N
requestO V
,V W
CancellationTokenX i
cancellationTokenj {
){ |
{ 
_logger 
. 
LogInformation 
( 
$str G
,G H
requestI P
.P Q
SearchQ W
)W X
;X Y
try 
{ 	
return 
await "
_clienteReadRepository /
./ 0
SearchByNameAsync0 A
(A B
requestB I
.I J
SearchJ P
,P Q
cancellationTokenR c
)c d
;d e
} 	
catch 
( 
	Exception 
ex 
) 
{ 	
_logger 
. 
LogError 
( 
ex 
,  
$str! P
,P Q
requestR Y
.Y Z
SearchZ `
)` a
;a b
throw 
; 
} 	
}   
}!! Ø
ùC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Application\Clientes\Queries\SearchClientesByName\SearchClientesByNameQuery.cs
	namespace 	
Clientes
 
. 
Application 
. 
Clientes '
.' (
Queries( /
./ 0 
SearchClientesByName0 D
;D E
public 
record %
SearchClientesByNameQuery '
(' (
string( .
Search/ 5
)5 6
:7 8
IRequest9 A
<A B
IReadOnlyListB O
<O P
ClienteP W
>W X
>X Y
;Y Z¿
òC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Application\Clientes\Queries\GetClienteById\GetClienteByIdQueryHandler.cs
	namespace 	
Clientes
 
. 
Application 
. 
Clientes '
.' (
Queries( /
./ 0
GetClienteById0 >
;> ?
public 
class &
GetClienteByIdQueryHandler '
:( )
IRequestHandler* 9
<9 :
GetClienteByIdQuery: M
,M N
ClienteO V
?V W
>W X
{		 
private

 
readonly

 "
IClienteReadRepository

 +"
_clienteReadRepository

, B
;

B C
private 
readonly 
ILogger 
< &
GetClienteByIdQueryHandler 7
>7 8
_logger9 @
;@ A
public 
&
GetClienteByIdQueryHandler %
(% &"
IClienteReadRepository& <!
clienteReadRepository= R
,R S
ILoggerT [
<[ \&
GetClienteByIdQueryHandler\ v
>v w
loggerx ~
)~ 
{ "
_clienteReadRepository 
=  !
clienteReadRepository! 6
;6 7
_logger 
= 
logger 
; 
} 
public 

async 
Task 
< 
Cliente 
? 
> 
Handle  &
(& '
GetClienteByIdQuery' :
request; B
,B C
CancellationTokenD U
cancellationTokenV g
)g h
{ 
_logger 
. 
LogInformation 
( 
$str =
,= >
request? F
.F G
IdG I
)I J
;J K
try 
{ 	
return 
await "
_clienteReadRepository /
./ 0
GetByIdAsync0 <
(< =
request= D
.D E
IdE G
,G H
cancellationTokenI Z
)Z [
;[ \
} 	
catch 
( 
	Exception 
ex 
) 
{ 	
_logger 
. 
LogError 
( 
ex 
,  
$str! J
,J K
requestL S
.S T
IdT V
)V W
;W X
throw 
; 
} 	
}   
}!! Ê
ëC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Application\Clientes\Queries\GetClienteById\GetClienteByIdQuery.cs
	namespace 	
Clientes
 
. 
Application 
. 
Clientes '
.' (
Queries( /
./ 0
GetClienteById0 >
;> ?
public 
record 
GetClienteByIdQuery !
(! "
int" %
Id& (
)( )
:* +
IRequest, 4
<4 5
Cliente5 <
?< =
>= >
;> ? 
òC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Application\Clientes\Queries\GetAllClientes\GetAllClientesQueryHandler.cs
	namespace 	
Clientes
 
. 
Application 
. 
Clientes '
.' (
Queries( /
./ 0
GetAllClientes0 >
;> ?
public 
class &
GetAllClientesQueryHandler '
:( )
IRequestHandler* 9
<9 :
GetAllClientesQuery: M
,M N
IReadOnlyListO \
<\ ]
Cliente] d
>d e
>e f
{		 
private

 
readonly

 "
IClienteReadRepository

 +"
_clienteReadRepository

, B
;

B C
private 
readonly 
ILogger 
< &
GetAllClientesQueryHandler 7
>7 8
_logger9 @
;@ A
public 
&
GetAllClientesQueryHandler %
(% &"
IClienteReadRepository& <!
clienteReadRepository= R
,R S
ILoggerT [
<[ \&
GetAllClientesQueryHandler\ v
>v w
loggerx ~
)~ 
{ "
_clienteReadRepository 
=  !
clienteReadRepository! 6
;6 7
_logger 
= 
logger 
; 
} 
public 

async 
Task 
< 
IReadOnlyList #
<# $
Cliente$ +
>+ ,
>, -
Handle. 4
(4 5
GetAllClientesQuery5 H
requestI P
,P Q
CancellationTokenR c
cancellationTokend u
)u v
{ 
_logger 
. 
LogInformation 
( 
$str I
)I J
;J K
try 
{ 	
return 
await "
_clienteReadRepository /
./ 0
GetAllAsync0 ;
(; <
cancellationToken< M
)M N
;N O
} 	
catch 
( 
	Exception 
ex 
) 
{ 	
_logger 
. 
LogError 
( 
ex 
,  
$str! J
)J K
;K L
throw 
; 
} 	
}   
}!! Ô
ëC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Application\Clientes\Queries\GetAllClientes\GetAllClientesQuery.cs
	namespace 	
Clientes
 
. 
Application 
. 
Clientes '
.' (
Queries( /
./ 0
GetAllClientes0 >
;> ?
public 
record 
GetAllClientesQuery !
(! "
)" #
:$ %
IRequest& .
<. /
IReadOnlyList/ <
<< =
Cliente= D
>D E
>E F
;F G»
âC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Application\Clientes\Interfaces\IClienteWriteRepository.cs
	namespace 	
Clientes
 
. 
Application 
. 
Clientes '
.' (

Interfaces( 2
;2 3
public 
	interface #
IClienteWriteRepository (
{ 
Task 
< 	
Cliente	 
> 
InsertAsync 
( 
string 
nombre 
, 
string		 
apellido		 
,		 
string

 
razonSocial

 
,

 
string 
cuit 
, 
DateOnly 
fechaNacimiento  
,  !
string 
telefonoCelular 
, 
string 
email 
, 
CancellationToken 
cancellationToken +
=, -
default. 5
)5 6
;6 7
Task 
< 	
Cliente	 
> 
UpdateAsync 
( 
int 
id 
, 
string 
nombre 
, 
string 
apellido 
, 
string 
razonSocial 
, 
string 
cuit 
, 
DateOnly 
fechaNacimiento  
,  !
string 
telefonoCelular 
, 
string 
email 
, 
CancellationToken 
cancellationToken +
=, -
default. 5
)5 6
;6 7
Task 
DeleteAsync	 
( 
int 
id 
, 
CancellationToken .
cancellationToken/ @
=A B
defaultC J
)J K
;K L
} ÷

àC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Application\Clientes\Interfaces\IClienteReadRepository.cs
	namespace 	
Clientes
 
. 
Application 
. 
Clientes '
.' (

Interfaces( 2
;2 3
public 
	interface "
IClienteReadRepository '
{ 
Task 
< 	
IReadOnlyList	 
< 
Cliente 
> 
>  
GetAllAsync! ,
(, -
CancellationToken- >
cancellationToken? P
=Q R
defaultS Z
)Z [
;[ \
Task		 
<		 	
Cliente			 
?		 
>		 
GetByIdAsync		 
(		  
int		  #
id		$ &
,		& '
CancellationToken		( 9
cancellationToken		: K
=		L M
default		N U
)		U V
;		V W
Task 
< 	
IReadOnlyList	 
< 
Cliente 
> 
>  
SearchByNameAsync! 2
(2 3
string3 9
search: @
,@ A
CancellationTokenB S
cancellationTokenT e
=f g
defaulth o
)o p
;p q
} π
äC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Application\Clientes\Exceptions\ClienteNotFoundException.cs
	namespace 	
Clientes
 
. 
Application 
. 
Clientes '
.' (

Exceptions( 2
;2 3
public 
class $
ClienteNotFoundException %
:& '
	Exception( 1
{ 
public 
$
ClienteNotFoundException #
(# $
string$ *
message+ 2
)2 3
:4 5
base6 :
(: ;
message; B
)B C
{ 
} 
} π
äC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Application\Clientes\Exceptions\ClienteConflictException.cs
	namespace 	
Clientes
 
. 
Application 
. 
Clientes '
.' (

Exceptions( 2
;2 3
public 
class $
ClienteConflictException %
:& '
	Exception( 1
{ 
public 
$
ClienteConflictException #
(# $
string$ *
message+ 2
)2 3
:4 5
base6 :
(: ;
message; B
)B C
{ 
} 
} ˛
ôC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Application\Clientes\Commands\UpdateCliente\UpdateClienteCommandHandler.cs
	namespace 	
Clientes
 
. 
Application 
. 
Clientes '
.' (
Commands( 0
.0 1
UpdateCliente1 >
;> ?
public		 
class		 '
UpdateClienteCommandHandler		 (
:		) *
IRequestHandler		+ :
<		: ; 
UpdateClienteCommand		; O
,		O P
Cliente		Q X
>		X Y
{

 
private 
readonly #
IClienteWriteRepository ,#
_clienteWriteRepository- D
;D E
private 
readonly 
ILogger 
< '
UpdateClienteCommandHandler 8
>8 9
_logger: A
;A B
public 
'
UpdateClienteCommandHandler &
(& '#
IClienteWriteRepository' >"
clienteWriteRepository? U
,U V
ILoggerW ^
<^ _'
UpdateClienteCommandHandler_ z
>z {
logger	| Ç
)
Ç É
{ #
_clienteWriteRepository 
=  !"
clienteWriteRepository" 8
;8 9
_logger 
= 
logger 
; 
} 
public 

async 
Task 
< 
Cliente 
> 
Handle %
(% & 
UpdateClienteCommand& :
request; B
,B C
CancellationTokenD U
cancellationTokenV g
)g h
{ 
_logger 
. 
LogInformation 
( 
$str :
,: ;
request< C
.C D
IdD F
)F G
;G H
try 
{ 	
var 
cliente 
= 
await #
_clienteWriteRepository  7
.7 8
UpdateAsync8 C
(C D
request 
. 
Id 
, 
request 
. 
Nombre 
, 
request 
. 
Apellido  
,  !
request 
. 
RazonSocial #
,# $
request 
. 
Cuit 
, 
request   
.   
FechaNacimiento   '
,  ' (
request!! 
.!! 
TelefonoCelular!! '
,!!' (
request"" 
."" 
Email"" 
,"" 
cancellationToken## !
)##! "
;##" #
_logger%% 
.%% 
LogInformation%% "
(%%" #
$str%%# K
,%%K L
request%%M T
.%%T U
Id%%U W
)%%W X
;%%X Y
return'' 
cliente'' 
;'' 
}(( 	
catch)) 
()) $
ClienteConflictException)) '
)))' (
{** 	
_logger++ 
.++ 

LogWarning++ 
(++ 
$str++ H
,++H I
request++J Q
.++Q R
Id++R T
)++T U
;++U V
throw,, 
;,, 
}-- 	
catch.. 
(.. $
ClienteNotFoundException.. '
)..' (
{// 	
_logger00 
.00 

LogWarning00 
(00 
$str00 O
,00O P
request00Q X
.00X Y
Id00Y [
)00[ \
;00\ ]
throw11 
;11 
}22 	
catch33 
(33 
	Exception33 
ex33 
)33 
{44 	
_logger55 
.55 
LogError55 
(55 
ex55 
,55  
$str55! Q
,55Q R
request55S Z
.55Z [
Id55[ ]
)55] ^
;55^ _
throw66 
;66 
}77 	
}88 
}99 ∫
íC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Application\Clientes\Commands\UpdateCliente\UpdateClienteCommand.cs
	namespace 	
Clientes
 
. 
Application 
. 
Clientes '
.' (
Commands( 0
.0 1
UpdateCliente1 >
;> ?
public 
record  
UpdateClienteCommand "
(" #
int 
Id 

,
 
string 

Nombre 
, 
string 

Apellido 
, 
string		 

RazonSocial		 
,		 
string

 

Cuit

 
,

 
DateOnly 
FechaNacimiento 
, 
string 

TelefonoCelular 
, 
string 

Email 
) 
: 
IRequest 
< 
Domain #
.# $
Entities$ ,
., -
Cliente- 4
>4 5
;5 6Â
ôC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Application\Clientes\Commands\DeleteCliente\DeleteClienteCommandHandler.cs
	namespace 	
Clientes
 
. 
Application 
. 
Clientes '
.' (
Commands( 0
.0 1
DeleteCliente1 >
;> ?
public 
class '
DeleteClienteCommandHandler (
:) *
IRequestHandler+ :
<: ; 
DeleteClienteCommand; O
>O P
{		 
private

 
readonly

 #
IClienteWriteRepository

 ,#
_clienteWriteRepository

- D
;

D E
private 
readonly 
ILogger 
< '
DeleteClienteCommandHandler 8
>8 9
_logger: A
;A B
public 
'
DeleteClienteCommandHandler &
(& '#
IClienteWriteRepository' >"
clienteWriteRepository? U
,U V
ILoggerW ^
<^ _'
DeleteClienteCommandHandler_ z
>z {
logger	| Ç
)
Ç É
{ #
_clienteWriteRepository 
=  !"
clienteWriteRepository" 8
;8 9
_logger 
= 
logger 
; 
} 
public 

async 
Task 
< 
Unit 
> 
Handle "
(" # 
DeleteClienteCommand# 7
request8 ?
,? @
CancellationTokenA R
cancellationTokenS d
)d e
{ 
_logger 
. 
LogInformation 
( 
$str 8
,8 9
request: A
.A B
IdB D
)D E
;E F
try 
{ 	
await #
_clienteWriteRepository )
.) *
DeleteAsync* 5
(5 6
request6 =
.= >
Id> @
,@ A
cancellationTokenB S
)S T
;T U
_logger 
. 
LogInformation "
(" #
$str# I
,I J
requestK R
.R S
IdS U
)U V
;V W
return 
Unit 
. 
Value 
; 
} 	
catch 
( $
ClienteNotFoundException '
)' (
{ 	
_logger 
. 

LogWarning 
( 
$str M
,M N
requestO V
.V W
IdW Y
)Y Z
;Z [
throw   
;   
}!! 	
catch"" 
("" 
	Exception"" 
ex"" 
)"" 
{## 	
_logger$$ 
.$$ 
LogError$$ 
($$ 
ex$$ 
,$$  
$str$$! O
,$$O P
request$$Q X
.$$X Y
Id$$Y [
)$$[ \
;$$\ ]
throw%% 
;%% 
}&& 	
}'' 
}(( ¶
íC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Application\Clientes\Commands\DeleteCliente\DeleteClienteCommand.cs
	namespace 	
Clientes
 
. 
Application 
. 
Clientes '
.' (
Commands( 0
.0 1
DeleteCliente1 >
;> ?
public 
record  
DeleteClienteCommand "
(" #
int# &
Id' )
)) *
:+ ,
IRequest- 5
;5 6È
ôC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Application\Clientes\Commands\CreateCliente\CreateClienteCommandHandler.cs
	namespace 	
Clientes
 
. 
Application 
. 
Clientes '
.' (
Commands( 0
.0 1
CreateCliente1 >
;> ?
public		 
class		 '
CreateClienteCommandHandler		 (
:		) *
IRequestHandler		+ :
<		: ; 
CreateClienteCommand		; O
,		O P
Cliente		Q X
>		X Y
{

 
private 
readonly #
IClienteWriteRepository ,#
_clienteWriteRepository- D
;D E
private 
readonly 
ILogger 
< '
CreateClienteCommandHandler 8
>8 9
_logger: A
;A B
public 
'
CreateClienteCommandHandler &
(& '#
IClienteWriteRepository "
clienteWriteRepository  6
,6 7
ILogger 
< '
CreateClienteCommandHandler +
>+ ,
logger- 3
)3 4
{ #
_clienteWriteRepository 
=  !"
clienteWriteRepository" 8
;8 9
_logger 
= 
logger 
; 
} 
public 

async 
Task 
< 
Cliente 
> 
Handle %
(% & 
CreateClienteCommand& :
request; B
,B C
CancellationTokenD U
cancellationTokenV g
)g h
{ 
_logger 
. 
LogInformation 
( 
$str B
,B C
requestD K
.K L
CuitL P
)P Q
;Q R
try 
{ 	
var 
cliente 
= 
await #
_clienteWriteRepository  7
.7 8
InsertAsync8 C
(C D
request 
. 
Nombre 
, 
request 
. 
Apellido  
,  !
request 
. 
RazonSocial #
,# $
request   
.   
Cuit   
,   
request!! 
.!! 
FechaNacimiento!! '
,!!' (
request"" 
."" 
TelefonoCelular"" '
,""' (
request## 
.## 
Email## 
,## 
cancellationToken$$ !
)$$! "
;$$" #
_logger&& 
.&& 
LogInformation&& "
(&&" #
$str&&# ?
,&&? @
cliente&&A H
.&&H I
Id&&I K
)&&K L
;&&L M
return(( 
cliente(( 
;(( 
})) 	
catch** 
(** $
ClienteConflictException** '
)**' (
{++ 	
_logger,, 
.,, 

LogWarning,, 
(,, 
$str,, B
,,,B C
request,,D K
.,,K L
Cuit,,L P
),,P Q
;,,Q R
throw-- 
;-- 
}.. 	
catch// 
(// 
	Exception// 
ex// 
)// 
{00 	
_logger11 
.11 
LogError11 
(11 
ex11 
,11  
$str11! K
,11K L
request11M T
.11T U
Cuit11U Y
)11Y Z
;11Z [
throw22 
;22 
}33 	
}44 
}55 à
íC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Application\Clientes\Commands\CreateCliente\CreateClienteCommand.cs
	namespace 	
Clientes
 
. 
Application 
. 
Clientes '
.' (
Commands( 0
.0 1
CreateCliente1 >
;> ?
public 
record  
CreateClienteCommand "
(" #
string 

Nombre 
, 
string 

Apellido 
, 
string 

RazonSocial 
, 
string		 

Cuit		 
,		 
DateOnly

 
FechaNacimiento

 
,

 
string 

TelefonoCelular 
, 
string 

Email 
) 
: 
IRequest 
< 
Domain 
. 
Entities 
. 
Cliente $
>$ %
;% &