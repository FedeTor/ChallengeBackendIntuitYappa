ко
ДC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Infrastructure\Repositories\ClienteWriteRepository.cs
	namespace 	
Clientes
 
. 
Infrastructure !
.! "
Repositories" .
;. /
public 
class "
ClienteWriteRepository #
:$ %#
IClienteWriteRepository& =
{ 
private 
readonly 
string 
? 
_connectionString .
;. /
private 
readonly $
INpgsqlConnectionFactory -
_connectionFactory. @
;@ A
private 
readonly 
ILogger 
< "
ClienteWriteRepository 3
>3 4
_logger5 <
;< =
public 
"
ClienteWriteRepository !
(! "
IConfiguration 
configuration $
,$ %
ILogger 
< "
ClienteWriteRepository &
>& '
logger( .
,. /$
INpgsqlConnectionFactory  
connectionFactory! 2
)2 3
{ 
_connectionString 
= 
configuration )
.) *
GetConnectionString* =
(= >
$str> Q
)Q R
;R S
_logger 
= 
logger 
; 
_connectionFactory 
= 
connectionFactory .
;. /
} 
public 

async 
Task 
< 
Cliente 
> 
InsertAsync *
(* +
string 
nombre 
, 
string 
apellido 
, 
string   
razonSocial   
,   
string!! 
cuit!! 
,!! 
DateOnly"" 
fechaNacimiento""  
,""  !
string## 
telefonoCelular## 
,## 
string$$ 
email$$ 
,$$ 
CancellationToken%% 
cancellationToken%% +
=%%, -
default%%. 5
)%%5 6
{&& 
if'' 

('' 
string'' 
.'' 
IsNullOrWhiteSpace'' %
(''% &
_connectionString''& 7
)''7 8
)''8 9
{(( 	
throw)) 
new)) %
InvalidOperationException)) /
())/ 0
$str))0 j
)))j k
;))k l
}** 	
try,, 
{-- 	
await.. 
using.. 
var.. 

connection.. &
=..' (
_connectionFactory..) ;
...; <
CreateConnection..< L
(..L M
_connectionString..M ^
)..^ _
;.._ `
await// 

connection// 
.// 
	OpenAsync// &
(//& '
cancellationToken//' 8
)//8 9
;//9 :
await11 
using11 
(11 
var11 
command11 $
=11% &

connection11' 1
.111 2
CreateCommand112 ?
(11? @
$str11@ T
)11T U
)11U V
{22 
command33 
.33 
CommandType33 #
=33$ %
CommandType33& 1
.331 2
StoredProcedure332 A
;33A B
command44 
.44 
AddParameter44 $
(44$ %
new44% (
NpgsqlParameter44) 8
(448 9
$str449 C
,44C D
nombre44E K
)44K L
)44L M
;44M N
command55 
.55 
AddParameter55 $
(55$ %
new55% (
NpgsqlParameter55) 8
(558 9
$str559 E
,55E F
apellido55G O
)55O P
)55P Q
;55Q R
command66 
.66 
AddParameter66 $
(66$ %
new66% (
NpgsqlParameter66) 8
(668 9
$str669 I
,66I J
razonSocial66K V
)66V W
)66W X
;66X Y
command77 
.77 
AddParameter77 $
(77$ %
new77% (
NpgsqlParameter77) 8
(778 9
$str779 A
,77A B
cuit77C G
)77G H
)77H I
;77I J
command88 
.88 
AddParameter88 $
(88$ %
new88% (
NpgsqlParameter88) 8
(888 9
$str889 M
,88M N
fechaNacimiento88O ^
)88^ _
)88_ `
;88` a
command99 
.99 
AddParameter99 $
(99$ %
new99% (
NpgsqlParameter99) 8
(998 9
$str999 M
,99M N
telefonoCelular99O ^
)99^ _
)99_ `
;99` a
command:: 
.:: 
AddParameter:: $
(::$ %
new::% (
NpgsqlParameter::) 8
(::8 9
$str::9 B
,::B C
email::D I
)::I J
)::J K
;::K L
var<< 
idParameter<< 
=<<  !
new<<" %
NpgsqlParameter<<& 5
(<<5 6
$str<<6 <
,<<< =
NpgsqlDbType<<> J
.<<J K
Integer<<K R
)<<R S
{== 
	Direction>> 
=>> 
ParameterDirection>>  2
.>>2 3
Output>>3 9
}?? 
;?? 
command@@ 
.@@ 
AddParameter@@ $
(@@$ %
idParameter@@% 0
)@@0 1
;@@1 2
awaitBB 
commandBB 
.BB  
ExecuteNonQueryAsyncBB 2
(BB2 3
cancellationTokenBB3 D
)BBD E
;BBE F
ifDD 
(DD 
idParameterDD 
.DD  
ValueDD  %
isDD& (
DBNullDD) /
)DD/ 0
{EE 
throwFF 
newFF %
InvalidOperationExceptionFF 7
(FF7 8
$strFF8 z
)FFz {
;FF{ |
}GG 
varII 
newIdII 
=II 
ConvertII #
.II# $
ToInt32II$ +
(II+ ,
idParameterII, 7
.II7 8
ValueII8 =
)II= >
;II> ?
returnKK 
awaitKK 
GetByIdAsyncKK )
(KK) *

connectionKK* 4
,KK4 5
newIdKK6 ;
,KK; <
cancellationTokenKK= N
)KKN O
;KKO P
}LL 
}MM 	
catchNN 
(NN 
PostgresExceptionNN  
exNN! #
)NN# $
{OO 	
_loggerPP 
.PP 

LogWarningPP 
(PP 
exPP !
,PP! "
$strPP# V
,PPV W
cuitPPX \
)PP\ ]
;PP] ^
ifRR 
(RR 
exRR 
.RR 
MessageRR 
.RR 
ContainsRR #
(RR# $
$strRR$ F
,RRF G
StringComparisonRRH X
.RRX Y&
InvariantCultureIgnoreCaseRRY s
)RRs t
||RRu w
exSS 
.SS 
MessageSS 
.SS 
ContainsSS #
(SS# $
$strSS$ G
,SSG H
StringComparisonSSI Y
.SSY Z&
InvariantCultureIgnoreCaseSSZ t
)SSt u
)SSu v
{TT 
throwUU 
newUU $
ClienteConflictExceptionUU 2
(UU2 3
exUU3 5
.UU5 6
MessageUU6 =
)UU= >
;UU> ?
}VV 
throwXX 
;XX 
}YY 	
catchZZ 
(ZZ 
	ExceptionZZ 
exZZ 
)ZZ 
{[[ 	
_logger\\ 
.\\ 
LogError\\ 
(\\ 
ex\\ 
,\\  
$str\\! N
,\\N O
cuit\\P T
)\\T U
;\\U V
throw]] 
;]] 
}^^ 	
}__ 
publicaa 

asyncaa 
Taskaa 
<aa 
Clienteaa 
>aa 
UpdateAsyncaa *
(aa* +
intbb 
idbb 
,bb 
stringcc 
nombrecc 
,cc 
stringdd 
apellidodd 
,dd 
stringee 
razonSocialee 
,ee 
stringff 
cuitff 
,ff 
DateOnlygg 
fechaNacimientogg  
,gg  !
stringhh 
telefonoCelularhh 
,hh 
stringii 
emailii 
,ii 
CancellationTokenjj 
cancellationTokenjj +
=jj, -
defaultjj. 5
)jj5 6
{kk 
ifll 

(ll 
stringll 
.ll 
IsNullOrWhiteSpacell %
(ll% &
_connectionStringll& 7
)ll7 8
)ll8 9
{mm 	
thrownn 
newnn %
InvalidOperationExceptionnn /
(nn/ 0
$strnn0 j
)nnj k
;nnk l
}oo 	
tryqq 
{rr 	
awaitss 
usingss 
varss 

connectionss &
=ss' (
_connectionFactoryss) ;
.ss; <
CreateConnectionss< L
(ssL M
_connectionStringssM ^
)ss^ _
;ss_ `
awaittt 

connectiontt 
.tt 
	OpenAsynctt &
(tt& '
cancellationTokentt' 8
)tt8 9
;tt9 :
awaitvv 
usingvv 
(vv 
varvv 
commandvv $
=vv% &

connectionvv' 1
.vv1 2
CreateCommandvv2 ?
(vv? @
$strvv@ T
)vvT U
)vvU V
{ww 
commandxx 
.xx 
CommandTypexx #
=xx$ %
CommandTypexx& 1
.xx1 2
StoredProcedurexx2 A
;xxA B
commandyy 
.yy 
AddParameteryy $
(yy$ %
newyy% (
NpgsqlParameteryy) 8
(yy8 9
$stryy9 ?
,yy? @
idyyA C
)yyC D
)yyD E
;yyE F
commandzz 
.zz 
AddParameterzz $
(zz$ %
newzz% (
NpgsqlParameterzz) 8
(zz8 9
$strzz9 C
,zzC D
nombrezzE K
)zzK L
)zzL M
;zzM N
command{{ 
.{{ 
AddParameter{{ $
({{$ %
new{{% (
NpgsqlParameter{{) 8
({{8 9
$str{{9 E
,{{E F
apellido{{G O
){{O P
){{P Q
;{{Q R
command|| 
.|| 
AddParameter|| $
(||$ %
new||% (
NpgsqlParameter||) 8
(||8 9
$str||9 I
,||I J
razonSocial||K V
)||V W
)||W X
;||X Y
command}} 
.}} 
AddParameter}} $
(}}$ %
new}}% (
NpgsqlParameter}}) 8
(}}8 9
$str}}9 A
,}}A B
cuit}}C G
)}}G H
)}}H I
;}}I J
command~~ 
.~~ 
AddParameter~~ $
(~~$ %
new~~% (
NpgsqlParameter~~) 8
(~~8 9
$str~~9 M
,~~M N
fechaNacimiento~~O ^
)~~^ _
)~~_ `
;~~` a
command 
. 
AddParameter $
($ %
new% (
NpgsqlParameter) 8
(8 9
$str9 M
,M N
telefonoCelularO ^
)^ _
)_ `
;` a
command
АА 
.
АА 
AddParameter
АА $
(
АА$ %
new
АА% (
NpgsqlParameter
АА) 8
(
АА8 9
$str
АА9 B
,
ААB C
email
ААD I
)
ААI J
)
ААJ K
;
ААK L
var
ВВ 
rowsParameter
ВВ !
=
ВВ" #
new
ВВ$ '
NpgsqlParameter
ВВ( 7
(
ВВ7 8
$str
ВВ8 I
,
ВВI J
NpgsqlDbType
ВВK W
.
ВВW X
Integer
ВВX _
)
ВВ_ `
{
ГГ 
	Direction
ДД 
=
ДД  
ParameterDirection
ДД  2
.
ДД2 3
Output
ДД3 9
}
ЕЕ 
;
ЕЕ 
command
ЖЖ 
.
ЖЖ 
AddParameter
ЖЖ $
(
ЖЖ$ %
rowsParameter
ЖЖ% 2
)
ЖЖ2 3
;
ЖЖ3 4
await
ИИ 
command
ИИ 
.
ИИ "
ExecuteNonQueryAsync
ИИ 2
(
ИИ2 3
cancellationToken
ИИ3 D
)
ИИD E
;
ИИE F
if
КК 
(
КК 
rowsParameter
КК !
.
КК! "
Value
КК" '
is
КК( *
DBNull
КК+ 1
||
КК2 4
Convert
КК5 <
.
КК< =
ToInt32
КК= D
(
ККD E
rowsParameter
ККE R
.
ККR S
Value
ККS X
)
ККX Y
==
ККZ \
$num
КК] ^
)
КК^ _
{
ЛЛ 
throw
ММ 
new
ММ &
ClienteNotFoundException
ММ 6
(
ММ6 7
$"
ММ7 9
$str
ММ9 X
{
ММX Y
id
ММY [
}
ММ[ \
"
ММ\ ]
)
ММ] ^
;
ММ^ _
}
НН 
}
ОО 
return
РР 
await
РР 
GetByIdAsync
РР %
(
РР% &

connection
РР& 0
,
РР0 1
id
РР2 4
,
РР4 5
cancellationToken
РР6 G
)
РРG H
;
РРH I
}
СС 	
catch
ТТ 
(
ТТ 
PostgresException
ТТ  
ex
ТТ! #
)
ТТ# $
{
УУ 	
_logger
ФФ 
.
ФФ 

LogWarning
ФФ 
(
ФФ 
ex
ФФ !
,
ФФ! "
$str
ФФ# V
,
ФФV W
id
ФФX Z
)
ФФZ [
;
ФФ[ \
if
ЦЦ 
(
ЦЦ 
ex
ЦЦ 
.
ЦЦ 
Message
ЦЦ 
.
ЦЦ 
Contains
ЦЦ #
(
ЦЦ# $
$str
ЦЦ$ F
,
ЦЦF G
StringComparison
ЦЦH X
.
ЦЦX Y(
InvariantCultureIgnoreCase
ЦЦY s
)
ЦЦs t
||
ЦЦu w
ex
ЧЧ 
.
ЧЧ 
Message
ЧЧ 
.
ЧЧ 
Contains
ЧЧ #
(
ЧЧ# $
$str
ЧЧ$ G
,
ЧЧG H
StringComparison
ЧЧI Y
.
ЧЧY Z(
InvariantCultureIgnoreCase
ЧЧZ t
)
ЧЧt u
)
ЧЧu v
{
ШШ 
throw
ЩЩ 
new
ЩЩ &
ClienteConflictException
ЩЩ 2
(
ЩЩ2 3
ex
ЩЩ3 5
.
ЩЩ5 6
Message
ЩЩ6 =
)
ЩЩ= >
;
ЩЩ> ?
}
ЪЪ 
if
ЬЬ 
(
ЬЬ 
ex
ЬЬ 
.
ЬЬ 
Message
ЬЬ 
.
ЬЬ 
Contains
ЬЬ #
(
ЬЬ# $
$str
ЬЬ$ D
,
ЬЬD E
StringComparison
ЬЬF V
.
ЬЬV W(
InvariantCultureIgnoreCase
ЬЬW q
)
ЬЬq r
)
ЬЬr s
{
ЭЭ 
throw
ЮЮ 
new
ЮЮ &
ClienteNotFoundException
ЮЮ 2
(
ЮЮ2 3
ex
ЮЮ3 5
.
ЮЮ5 6
Message
ЮЮ6 =
)
ЮЮ= >
;
ЮЮ> ?
}
ЯЯ 
throw
°° 
;
°° 
}
ҐҐ 	
catch
££ 
(
££ &
ClienteNotFoundException
££ '
)
££' (
{
§§ 	
throw
•• 
;
•• 
}
¶¶ 	
catch
ІІ 
(
ІІ 
	Exception
ІІ 
ex
ІІ 
)
ІІ 
{
®® 	
_logger
©© 
.
©© 
LogError
©© 
(
©© 
ex
©© 
,
©©  
$str
©©! N
,
©©N O
id
©©P R
)
©©R S
;
©©S T
throw
™™ 
;
™™ 
}
ЂЂ 	
}
ђђ 
public
ЃЃ 

async
ЃЃ 
Task
ЃЃ 
DeleteAsync
ЃЃ !
(
ЃЃ! "
int
ЃЃ" %
id
ЃЃ& (
,
ЃЃ( )
CancellationToken
ЃЃ* ;
cancellationToken
ЃЃ< M
=
ЃЃN O
default
ЃЃP W
)
ЃЃW X
{
ѓѓ 
if
∞∞ 

(
∞∞ 
string
∞∞ 
.
∞∞  
IsNullOrWhiteSpace
∞∞ %
(
∞∞% &
_connectionString
∞∞& 7
)
∞∞7 8
)
∞∞8 9
{
±± 	
throw
≤≤ 
new
≤≤ '
InvalidOperationException
≤≤ /
(
≤≤/ 0
$str
≤≤0 j
)
≤≤j k
;
≤≤k l
}
≥≥ 	
try
µµ 
{
ґґ 	
await
ЈЈ 
using
ЈЈ 
var
ЈЈ 

connection
ЈЈ &
=
ЈЈ' ( 
_connectionFactory
ЈЈ) ;
.
ЈЈ; <
CreateConnection
ЈЈ< L
(
ЈЈL M
_connectionString
ЈЈM ^
)
ЈЈ^ _
;
ЈЈ_ `
await
ЄЄ 

connection
ЄЄ 
.
ЄЄ 
	OpenAsync
ЄЄ &
(
ЄЄ& '
cancellationToken
ЄЄ' 8
)
ЄЄ8 9
;
ЄЄ9 :
await
ЇЇ 
using
ЇЇ 
var
ЇЇ 
command
ЇЇ #
=
ЇЇ$ %

connection
ЇЇ& 0
.
ЇЇ0 1
CreateCommand
ЇЇ1 >
(
ЇЇ> ?
$str
ЇЇ? S
)
ЇЇS T
;
ЇЇT U
command
її 
.
її 
CommandType
її 
=
її  !
CommandType
її" -
.
її- .
StoredProcedure
її. =
;
її= >
command
љљ 
.
љљ 
AddParameter
љљ  
(
љљ  !
new
љљ! $
NpgsqlParameter
љљ% 4
(
љљ4 5
$str
љљ5 ;
,
љљ; <
id
љљ= ?
)
љљ? @
)
љљ@ A
;
љљA B
var
њњ 
rowsParameter
њњ 
=
њњ 
new
њњ  #
NpgsqlParameter
њњ$ 3
(
њњ3 4
$str
њњ4 E
,
њњE F
NpgsqlDbType
њњG S
.
њњS T
Integer
њњT [
)
њњ[ \
{
јј 
	Direction
ЅЅ 
=
ЅЅ  
ParameterDirection
ЅЅ .
.
ЅЅ. /
Output
ЅЅ/ 5
}
¬¬ 
;
¬¬ 
command
√√ 
.
√√ 
AddParameter
√√  
(
√√  !
rowsParameter
√√! .
)
√√. /
;
√√/ 0
await
≈≈ 
command
≈≈ 
.
≈≈ "
ExecuteNonQueryAsync
≈≈ .
(
≈≈. /
cancellationToken
≈≈/ @
)
≈≈@ A
;
≈≈A B
if
«« 
(
«« 
rowsParameter
«« 
.
«« 
Value
«« #
is
««$ &
DBNull
««' -
||
««. 0
Convert
««1 8
.
««8 9
ToInt32
««9 @
(
««@ A
rowsParameter
««A N
.
««N O
Value
««O T
)
««T U
==
««V X
$num
««Y Z
)
««Z [
{
»» 
throw
…… 
new
…… &
ClienteNotFoundException
…… 2
(
……2 3
$"
……3 5
$str
……5 T
{
……T U
id
……U W
}
……W X
"
……X Y
)
……Y Z
;
……Z [
}
   
}
ЋЋ 	
catch
ћћ 
(
ћћ 
PostgresException
ћћ  
ex
ћћ! #
)
ћћ# $
{
ЌЌ 	
_logger
ќќ 
.
ќќ 

LogWarning
ќќ 
(
ќќ 
ex
ќќ !
,
ќќ! "
$str
ќќ# T
,
ќќT U
id
ќќV X
)
ќќX Y
;
ќќY Z
if
–– 
(
–– 
ex
–– 
.
–– 
Message
–– 
.
–– 
Contains
–– #
(
––# $
$str
––$ D
,
––D E
StringComparison
––F V
.
––V W(
InvariantCultureIgnoreCase
––W q
)
––q r
)
––r s
{
—— 
throw
““ 
new
““ &
ClienteNotFoundException
““ 2
(
““2 3
ex
““3 5
.
““5 6
Message
““6 =
)
““= >
;
““> ?
}
”” 
throw
’’ 
;
’’ 
}
÷÷ 	
catch
„„ 
(
„„ &
ClienteNotFoundException
„„ '
)
„„' (
{
ЎЎ 	
throw
ўў 
;
ўў 
}
ЏЏ 	
catch
џџ 
(
џџ 
	Exception
џџ 
ex
џџ 
)
џџ 
{
№№ 	
_logger
ЁЁ 
.
ЁЁ 
LogError
ЁЁ 
(
ЁЁ 
ex
ЁЁ 
,
ЁЁ  
$str
ЁЁ! L
,
ЁЁL M
id
ЁЁN P
)
ЁЁP Q
;
ЁЁQ R
throw
ёё 
;
ёё 
}
яя 	
}
аа 
private
вв 
static
вв 
async
вв 
Task
вв 
<
вв 
Cliente
вв %
>
вв% &
GetByIdAsync
вв' 3
(
вв3 4&
INpgsqlConnectionWrapper
вв4 L

connection
ввM W
,
ввW X
int
ввY \
id
вв] _
,
вв_ `
CancellationToken
ввa r 
cancellationTokenввs Д
)ввД Е
{
гг 
await
дд 
using
дд 
var
дд 
command
дд 
=
дд  !

connection
дд" ,
.
дд, -
CreateCommand
дд- :
(
дд: ;
$str
дд; R
)
ддR S
;
ддS T
command
ее 
.
ее 
CommandType
ее 
=
ее 
CommandType
ее )
.
ее) *
StoredProcedure
ее* 9
;
ее9 :
command
зз 
.
зз 
AddParameter
зз 
(
зз 
new
зз  
NpgsqlParameter
зз! 0
(
зз0 1
$str
зз1 7
,
зз7 8
id
зз9 ;
)
зз; <
)
зз< =
;
зз= >
var
йй 
outputParameters
йй 
=
йй 
new
йй "
List
йй# '
<
йй' (
NpgsqlParameter
йй( 7
>
йй7 8
{
кк 	
new
лл 
(
лл 
$str
лл 
,
лл 
NpgsqlDbType
лл $
.
лл$ %
Integer
лл% ,
)
лл, -
{
лл. /
	Direction
лл0 9
=
лл: ; 
ParameterDirection
лл< N
.
ллN O
Output
ллO U
}
ллV W
,
ллW X
new
мм 
(
мм 
$str
мм 
,
мм 
NpgsqlDbType
мм (
.
мм( )
Varchar
мм) 0
)
мм0 1
{
мм2 3
	Direction
мм4 =
=
мм> ? 
ParameterDirection
мм@ R
.
ммR S
Output
ммS Y
}
ммZ [
,
мм[ \
new
нн 
(
нн 
$str
нн 
,
нн 
NpgsqlDbType
нн *
.
нн* +
Varchar
нн+ 2
)
нн2 3
{
нн4 5
	Direction
нн6 ?
=
нн@ A 
ParameterDirection
ннB T
.
ннT U
Output
ннU [
}
нн\ ]
,
нн] ^
new
оо 
(
оо 
$str
оо  
,
оо  !
NpgsqlDbType
оо" .
.
оо. /
Varchar
оо/ 6
)
оо6 7
{
оо8 9
	Direction
оо: C
=
ооD E 
ParameterDirection
ооF X
.
ооX Y
Output
ооY _
}
оо` a
,
ооa b
new
пп 
(
пп 
$str
пп 
,
пп 
NpgsqlDbType
пп &
.
пп& '
Varchar
пп' .
)
пп. /
{
пп0 1
	Direction
пп2 ;
=
пп< = 
ParameterDirection
пп> P
.
ппP Q
Output
ппQ W
}
ппX Y
,
ппY Z
new
рр 
(
рр 
$str
рр $
,
рр$ %
NpgsqlDbType
рр& 2
.
рр2 3
Date
рр3 7
)
рр7 8
{
рр9 :
	Direction
рр; D
=
ррE F 
ParameterDirection
ррG Y
.
ррY Z
Output
ррZ `
}
ррa b
,
ррb c
new
сс 
(
сс 
$str
сс $
,
сс$ %
NpgsqlDbType
сс& 2
.
сс2 3
Varchar
сс3 :
)
сс: ;
{
сс< =
	Direction
сс> G
=
ссH I 
ParameterDirection
ссJ \
.
сс\ ]
Output
сс] c
}
ссd e
,
ссe f
new
тт 
(
тт 
$str
тт 
,
тт 
NpgsqlDbType
тт '
.
тт' (
Varchar
тт( /
)
тт/ 0
{
тт1 2
	Direction
тт3 <
=
тт= > 
ParameterDirection
тт? Q
.
ттQ R
Output
ттR X
}
ттY Z
,
ттZ [
new
уу 
(
уу 
$str
уу "
,
уу" #
NpgsqlDbType
уу$ 0
.
уу0 1
	Timestamp
уу1 :
)
уу: ;
{
уу< =
	Direction
уу> G
=
ууH I 
ParameterDirection
ууJ \
.
уу\ ]
Output
уу] c
}
ууd e
,
ууe f
new
фф 
(
фф 
$str
фф &
,
фф& '
NpgsqlDbType
фф( 4
.
фф4 5
	Timestamp
фф5 >
)
фф> ?
{
фф@ A
	Direction
ффB K
=
ффL M 
ParameterDirection
ффN `
.
фф` a
Output
ффa g
}
ффh i
}
хх 	
;
хх	 

command
чч 
.
чч 
AddParameters
чч 
(
чч 
outputParameters
чч .
)
чч. /
;
чч/ 0
await
щщ 
command
щщ 
.
щщ "
ExecuteNonQueryAsync
щщ *
(
щщ* +
cancellationToken
щщ+ <
)
щщ< =
;
щщ= >
if
ыы 

(
ыы 
command
ыы 
.
ыы 

Parameters
ыы 
[
ыы 
$str
ыы %
]
ыы% &
.
ыы& '
Value
ыы' ,
is
ыы- /
DBNull
ыы0 6
)
ыы6 7
{
ьь 	
throw
ээ 
new
ээ '
InvalidOperationException
ээ /
(
ээ/ 0
$"
ээ0 2
$str
ээ2 Y
{
ээY Z
id
ээZ \
}
ээ\ ]
$str
ээ] ^
"
ээ^ _
)
ээ_ `
;
ээ` a
}
юю 	
return
АА 

MapCliente
АА 
(
АА 
command
АА !
.
АА! "

Parameters
АА" ,
)
АА, -
;
АА- .
}
ББ 
private
ГГ 
static
ГГ 
Cliente
ГГ 

MapCliente
ГГ %
(
ГГ% &'
NpgsqlParameterCollection
ГГ& ?

parameters
ГГ@ J
)
ГГJ K
{
ДД 
return
ЕЕ 
new
ЕЕ 
Cliente
ЕЕ 
{
ЖЖ 	
Id
ЗЗ 
=
ЗЗ 
Convert
ЗЗ 
.
ЗЗ 
ToInt32
ЗЗ  
(
ЗЗ  !

parameters
ЗЗ! +
[
ЗЗ+ ,
$str
ЗЗ, 2
]
ЗЗ2 3
.
ЗЗ3 4
Value
ЗЗ4 9
)
ЗЗ9 :
,
ЗЗ: ;
Nombre
ИИ 
=
ИИ 
Convert
ИИ 
.
ИИ 
ToString
ИИ %
(
ИИ% &

parameters
ИИ& 0
[
ИИ0 1
$str
ИИ1 ;
]
ИИ; <
.
ИИ< =
Value
ИИ= B
)
ИИB C
??
ИИD F
string
ИИG M
.
ИИM N
Empty
ИИN S
,
ИИS T
Apellido
ЙЙ 
=
ЙЙ 
Convert
ЙЙ 
.
ЙЙ 
ToString
ЙЙ '
(
ЙЙ' (

parameters
ЙЙ( 2
[
ЙЙ2 3
$str
ЙЙ3 ?
]
ЙЙ? @
.
ЙЙ@ A
Value
ЙЙA F
)
ЙЙF G
??
ЙЙH J
string
ЙЙK Q
.
ЙЙQ R
Empty
ЙЙR W
,
ЙЙW X
RazonSocial
КК 
=
КК 
Convert
КК !
.
КК! "
ToString
КК" *
(
КК* +

parameters
КК+ 5
[
КК5 6
$str
КК6 F
]
ККF G
.
ККG H
Value
ККH M
)
ККM N
??
ККO Q
string
ККR X
.
ККX Y
Empty
ККY ^
,
КК^ _
Cuit
ЛЛ 
=
ЛЛ 
Convert
ЛЛ 
.
ЛЛ 
ToString
ЛЛ #
(
ЛЛ# $

parameters
ЛЛ$ .
[
ЛЛ. /
$str
ЛЛ/ 7
]
ЛЛ7 8
.
ЛЛ8 9
Value
ЛЛ9 >
)
ЛЛ> ?
??
ЛЛ@ B
string
ЛЛC I
.
ЛЛI J
Empty
ЛЛJ O
,
ЛЛO P
FechaNacimiento
ММ 
=
ММ 

ToDateOnly
ММ (
(
ММ( )

parameters
ММ) 3
[
ММ3 4
$str
ММ4 H
]
ММH I
.
ММI J
Value
ММJ O
)
ММO P
,
ММP Q
TelefonoCelular
НН 
=
НН 
Convert
НН %
.
НН% &
ToString
НН& .
(
НН. /

parameters
НН/ 9
[
НН9 :
$str
НН: N
]
ННN O
.
ННO P
Value
ННP U
)
ННU V
??
ННW Y
string
ННZ `
.
НН` a
Empty
ННa f
,
ННf g
Email
ОО 
=
ОО 
Convert
ОО 
.
ОО 
ToString
ОО $
(
ОО$ %

parameters
ОО% /
[
ОО/ 0
$str
ОО0 9
]
ОО9 :
.
ОО: ;
Value
ОО; @
)
ОО@ A
??
ООB D
string
ООE K
.
ООK L
Empty
ООL Q
,
ООQ R
FechaCreacion
ПП 
=
ПП 
Convert
ПП #
.
ПП# $

ToDateTime
ПП$ .
(
ПП. /

parameters
ПП/ 9
[
ПП9 :
$str
ПП: L
]
ППL M
.
ППM N
Value
ППN S
)
ППS T
,
ППT U
FechaModificacion
РР 
=
РР 
Convert
РР  '
.
РР' (

ToDateTime
РР( 2
(
РР2 3

parameters
РР3 =
[
РР= >
$str
РР> T
]
РРT U
.
РРU V
Value
РРV [
)
РР[ \
}
СС 	
;
СС	 

}
ТТ 
private
ФФ 
static
ФФ 
DateOnly
ФФ 

ToDateOnly
ФФ &
(
ФФ& '
object
ФФ' -
value
ФФ. 3
)
ФФ3 4
{
ХХ 
return
ЦЦ 
value
ЦЦ 
switch
ЦЦ 
{
ЧЧ 	
DateOnly
ШШ 
dateOnly
ШШ 
=>
ШШ  
dateOnly
ШШ! )
,
ШШ) *
DateTime
ЩЩ 
dateTime
ЩЩ 
=>
ЩЩ  
DateOnly
ЩЩ! )
.
ЩЩ) *
FromDateTime
ЩЩ* 6
(
ЩЩ6 7
dateTime
ЩЩ7 ?
)
ЩЩ? @
,
ЩЩ@ A
_
ЪЪ 
=>
ЪЪ 
DateOnly
ЪЪ 
.
ЪЪ 
FromDateTime
ЪЪ &
(
ЪЪ& '
Convert
ЪЪ' .
.
ЪЪ. /

ToDateTime
ЪЪ/ 9
(
ЪЪ9 :
value
ЪЪ: ?
)
ЪЪ? @
)
ЪЪ@ A
}
ЫЫ 	
;
ЫЫ	 

}
ЬЬ 
}ЭЭ «…
ГC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Infrastructure\Repositories\ClienteReadRepository.cs
	namespace 	
Clientes
 
. 
Infrastructure !
.! "
Repositories" .
;. /
public 
class !
ClienteReadRepository "
:# $"
IClienteReadRepository% ;
{ 
private 
readonly 
string 
? 
_connectionString .
;. /
private 
readonly $
INpgsqlConnectionFactory -
_connectionFactory. @
;@ A
private 
readonly 
ILogger 
< !
ClienteReadRepository 2
>2 3
_logger4 ;
;; <
public 
!
ClienteReadRepository  
(  !
IConfiguration 
configuration $
,$ %
ILogger 
< !
ClienteReadRepository %
>% &
logger' -
,- .$
INpgsqlConnectionFactory  
connectionFactory! 2
)2 3
{ 
_connectionString 
= 
configuration )
.) *
GetConnectionString* =
(= >
$str> Q
)Q R
;R S
_logger 
= 
logger 
; 
_connectionFactory 
= 
connectionFactory .
;. /
} 
public 

async 
Task 
< 
IReadOnlyList #
<# $
Cliente$ +
>+ ,
>, -
GetAllAsync. 9
(9 :
CancellationToken: K
cancellationTokenL ]
=^ _
default` g
)g h
{ 
if 

( 
string 
. 
IsNullOrWhiteSpace %
(% &
_connectionString& 7
)7 8
)8 9
{   	
throw!! 
new!! %
InvalidOperationException!! /
(!!/ 0
$str!!0 j
)!!j k
;!!k l
}"" 	
var$$ 
clientes$$ 
=$$ 
new$$ 
List$$ 
<$$  
Cliente$$  '
>$$' (
($$( )
)$$) *
;$$* +
try&& 
{'' 	
await(( 
using(( 
var(( 

connection(( &
=((' (
_connectionFactory(() ;
.((; <
CreateConnection((< L
(((L M
_connectionString((M ^
)((^ _
;((_ `
await)) 

connection)) 
.)) 
	OpenAsync)) &
())& '
cancellationToken))' 8
)))8 9
;))9 :
await** 
using** 
var** 
transaction** '
=**( )
await*** /

connection**0 :
.**: ;!
BeginTransactionAsync**; P
(**P Q
cancellationToken**Q b
)**b c
;**c d
await,, 
using,, 
(,, 
var,, 
command,, $
=,,% &

connection,,' 1
.,,1 2
CreateCommand,,2 ?
(,,? @
$str,,@ U
,,,U V
transaction,,W b
),,b c
),,c d
{-- 
command.. 
... 
CommandType.. #
=..$ %
CommandType..& 1
...1 2
StoredProcedure..2 A
;..A B
var00 
cursorParameter00 #
=00$ %
new00& )
NpgsqlParameter00* 9
(009 :
$str00: D
,00D E
NpgsqlDbType00F R
.00R S
	Refcursor00S \
)00\ ]
{11 
	Direction22 
=22 
ParameterDirection22  2
.222 3
InputOutput223 >
,22> ?
Value33 
=33 
$str33 .
}44 
;44 
command66 
.66 
AddParameter66 $
(66$ %
cursorParameter66% 4
)664 5
;665 6
await88 
command88 
.88  
ExecuteNonQueryAsync88 2
(882 3
cancellationToken883 D
)88D E
;88E F
var:: 

cursorName:: 
=::  
Convert::! (
.::( )
ToString::) 1
(::1 2
cursorParameter::2 A
.::A B
Value::B G
)::G H
??::I K
$str::L ^
;::^ _
await<< 
using<< 
var<< 
fetchCommand<<  ,
=<<- .

connection<</ 9
.<<9 :
CreateCommand<<: G
(<<G H
$"<<H J
$str<<J Y
{<<Y Z

cursorName<<Z d
}<<d e
"<<e f
,<<f g
transaction<<h s
)<<s t
;<<t u
await== 
using== 
var== 
reader==  &
===' (
await==) .
fetchCommand==/ ;
.==; <
ExecuteReaderAsync==< N
(==N O
cancellationToken==O `
)==` a
;==a b
while?? 
(?? 
await?? 
reader?? #
.??# $
	ReadAsync??$ -
(??- .
cancellationToken??. ?
)??? @
)??@ A
{@@ 
clientesAA 
.AA 
AddAA  
(AA  !

MapClienteAA! +
(AA+ ,
readerAA, 2
)AA2 3
)AA3 4
;AA4 5
}BB 
}CC 
awaitEE 
transactionEE 
.EE 
CommitAsyncEE )
(EE) *
cancellationTokenEE* ;
)EE; <
;EE< =
returnGG 
clientesGG 
;GG 
}HH 	
catchII 
(II 
	ExceptionII 
exII 
)II 
{JJ 	
_loggerKK 
.KK 
LogErrorKK 
(KK 
exKK 
,KK  
$strKK! F
)KKF G
;KKG H
throwLL 
;LL 
}MM 	
}NN 
publicPP 

asyncPP 
TaskPP 
<PP 
ClientePP 
?PP 
>PP 
GetByIdAsyncPP  ,
(PP, -
intPP- 0
idPP1 3
,PP3 4
CancellationTokenPP5 F
cancellationTokenPPG X
=PPY Z
defaultPP[ b
)PPb c
{QQ 
ifRR 

(RR 
stringRR 
.RR 
IsNullOrWhiteSpaceRR %
(RR% &
_connectionStringRR& 7
)RR7 8
)RR8 9
{SS 	
throwTT 
newTT %
InvalidOperationExceptionTT /
(TT/ 0
$strTT0 j
)TTj k
;TTk l
}UU 	
tryWW 
{XX 	
awaitYY 
usingYY 
varYY 

connectionYY &
=YY' (
_connectionFactoryYY) ;
.YY; <
CreateConnectionYY< L
(YYL M
_connectionStringYYM ^
)YY^ _
;YY_ `
awaitZZ 

connectionZZ 
.ZZ 
	OpenAsyncZZ &
(ZZ& '
cancellationTokenZZ' 8
)ZZ8 9
;ZZ9 :
await\\ 
using\\ 
var\\ 
command\\ #
=\\$ %

connection\\& 0
.\\0 1
CreateCommand\\1 >
(\\> ?
$str\\? V
)\\V W
;\\W X
command]] 
.]] 
CommandType]] 
=]]  !
CommandType]]" -
.]]- .
StoredProcedure]]. =
;]]= >
command__ 
.__ 
AddParameter__  
(__  !
new__! $
NpgsqlParameter__% 4
(__4 5
$str__5 ;
,__; <
id__= ?
)__? @
)__@ A
;__A B
varaa 
outputParametersaa  
=aa! "
newaa# &
Listaa' +
<aa+ ,
NpgsqlParameteraa, ;
>aa; <
{bb 
newcc 
(cc 
$strcc 
,cc 
NpgsqlDbTypecc (
.cc( )
Integercc) 0
)cc0 1
{cc2 3
	Directioncc4 =
=cc> ?
ParameterDirectioncc@ R
.ccR S
OutputccS Y
}ccZ [
,cc[ \
newdd 
(dd 
$strdd 
,dd 
NpgsqlDbTypedd  ,
.dd, -
Varchardd- 4
)dd4 5
{dd6 7
	Directiondd8 A
=ddB C
ParameterDirectionddD V
.ddV W
OutputddW ]
}dd^ _
,dd_ `
newee 
(ee 
$stree  
,ee  !
NpgsqlDbTypeee" .
.ee. /
Varcharee/ 6
)ee6 7
{ee8 9
	Directionee: C
=eeD E
ParameterDirectioneeF X
.eeX Y
OutputeeY _
}ee` a
,eea b
newff 
(ff 
$strff $
,ff$ %
NpgsqlDbTypeff& 2
.ff2 3
Varcharff3 :
)ff: ;
{ff< =
	Directionff> G
=ffH I
ParameterDirectionffJ \
.ff\ ]
Outputff] c
}ffd e
,ffe f
newgg 
(gg 
$strgg 
,gg 
NpgsqlDbTypegg *
.gg* +
Varchargg+ 2
)gg2 3
{gg4 5
	Directiongg6 ?
=gg@ A
ParameterDirectionggB T
.ggT U
OutputggU [
}gg\ ]
,gg] ^
newhh 
(hh 
$strhh (
,hh( )
NpgsqlDbTypehh* 6
.hh6 7
Datehh7 ;
)hh; <
{hh= >
	Directionhh? H
=hhI J
ParameterDirectionhhK ]
.hh] ^
Outputhh^ d
}hhe f
,hhf g
newii 
(ii 
$strii (
,ii( )
NpgsqlDbTypeii* 6
.ii6 7
Varcharii7 >
)ii> ?
{ii@ A
	DirectioniiB K
=iiL M
ParameterDirectioniiN `
.ii` a
Outputiia g
}iih i
,iii j
newjj 
(jj 
$strjj 
,jj 
NpgsqlDbTypejj +
.jj+ ,
Varcharjj, 3
)jj3 4
{jj5 6
	Directionjj7 @
=jjA B
ParameterDirectionjjC U
.jjU V
OutputjjV \
}jj] ^
,jj^ _
newkk 
(kk 
$strkk &
,kk& '
NpgsqlDbTypekk( 4
.kk4 5
	Timestampkk5 >
)kk> ?
{kk@ A
	DirectionkkB K
=kkL M
ParameterDirectionkkN `
.kk` a
Outputkka g
}kkh i
,kki j
newll 
(ll 
$strll *
,ll* +
NpgsqlDbTypell, 8
.ll8 9
	Timestampll9 B
)llB C
{llD E
	DirectionllF O
=llP Q
ParameterDirectionllR d
.lld e
Outputlle k
}lll m
}mm 
;mm 
commandoo 
.oo 
AddParametersoo !
(oo! "
outputParametersoo" 2
)oo2 3
;oo3 4
awaitqq 
commandqq 
.qq  
ExecuteNonQueryAsyncqq .
(qq. /
cancellationTokenqq/ @
)qq@ A
;qqA B
ifss 
(ss 
commandss 
.ss 

Parametersss "
[ss" #
$strss# )
]ss) *
.ss* +
Valuess+ 0
isss1 3
DBNullss4 :
)ss: ;
{tt 
returnuu 
nulluu 
;uu 
}vv 
returnxx 

MapClientexx 
(xx 
commandxx %
.xx% &

Parametersxx& 0
)xx0 1
;xx1 2
}yy 	
catchzz 
(zz 
PostgresExceptionzz  
exzz! #
)zz# $
whenzz% )
(zz* +
exzz+ -
.zz- .
Messagezz. 5
.zz5 6
Containszz6 >
(zz> ?
$strzz? Z
,zzZ [
StringComparisonzz\ l
.zzl m'
InvariantCultureIgnoreCase	zzm З
)
zzЗ И
)
zzИ Й
{{{ 	
_logger|| 
.|| 

LogWarning|| 
(|| 
ex|| !
,||! "
$str||# J
,||J K
id||L N
)||N O
;||O P
return}} 
null}} 
;}} 
}~~ 	
catch 
( 
	Exception 
ex 
) 
{
АА 	
_logger
ББ 
.
ББ 
LogError
ББ 
(
ББ 
ex
ББ 
,
ББ  
$str
ББ! J
,
ББJ K
id
ББL N
)
ББN O
;
ББO P
throw
ВВ 
;
ВВ 
}
ГГ 	
}
ДД 
public
ЖЖ 

async
ЖЖ 
Task
ЖЖ 
<
ЖЖ 
IReadOnlyList
ЖЖ #
<
ЖЖ# $
Cliente
ЖЖ$ +
>
ЖЖ+ ,
>
ЖЖ, -
SearchByNameAsync
ЖЖ. ?
(
ЖЖ? @
string
ЖЖ@ F
search
ЖЖG M
,
ЖЖM N
CancellationToken
ЖЖO `
cancellationToken
ЖЖa r
=
ЖЖs t
default
ЖЖu |
)
ЖЖ| }
{
ЗЗ 
if
ИИ 

(
ИИ 
string
ИИ 
.
ИИ  
IsNullOrWhiteSpace
ИИ %
(
ИИ% &
_connectionString
ИИ& 7
)
ИИ7 8
)
ИИ8 9
{
ЙЙ 	
throw
КК 
new
КК '
InvalidOperationException
КК /
(
КК/ 0
$str
КК0 j
)
ККj k
;
ККk l
}
ЛЛ 	
var
НН 
clientes
НН 
=
НН 
new
НН 
List
НН 
<
НН  
Cliente
НН  '
>
НН' (
(
НН( )
)
НН) *
;
НН* +
try
ПП 
{
РР 	
await
СС 
using
СС 
var
СС 

connection
СС &
=
СС' ( 
_connectionFactory
СС) ;
.
СС; <
CreateConnection
СС< L
(
ССL M
_connectionString
ССM ^
)
СС^ _
;
СС_ `
await
ТТ 

connection
ТТ 
.
ТТ 
	OpenAsync
ТТ &
(
ТТ& '
cancellationToken
ТТ' 8
)
ТТ8 9
;
ТТ9 :
await
УУ 
using
УУ 
var
УУ 
transaction
УУ '
=
УУ( )
await
УУ* /

connection
УУ0 :
.
УУ: ;#
BeginTransactionAsync
УУ; P
(
УУP Q
cancellationToken
УУQ b
)
УУb c
;
УУc d
await
ХХ 
using
ХХ 
(
ХХ 
var
ХХ 
command
ХХ $
=
ХХ% &

connection
ХХ' 1
.
ХХ1 2
CreateCommand
ХХ2 ?
(
ХХ? @
$str
ХХ@ \
,
ХХ\ ]
transaction
ХХ^ i
)
ХХi j
)
ХХj k
{
ЦЦ 
command
ЧЧ 
.
ЧЧ 
CommandType
ЧЧ #
=
ЧЧ$ %
CommandType
ЧЧ& 1
.
ЧЧ1 2
StoredProcedure
ЧЧ2 A
;
ЧЧA B
command
ЩЩ 
.
ЩЩ 
AddParameter
ЩЩ $
(
ЩЩ$ %
new
ЩЩ% (
NpgsqlParameter
ЩЩ) 8
(
ЩЩ8 9
$str
ЩЩ9 C
,
ЩЩC D
search
ЩЩE K
)
ЩЩK L
)
ЩЩL M
;
ЩЩM N
var
ЫЫ 
cursorParameter
ЫЫ #
=
ЫЫ$ %
new
ЫЫ& )
NpgsqlParameter
ЫЫ* 9
(
ЫЫ9 :
$str
ЫЫ: D
,
ЫЫD E
NpgsqlDbType
ЫЫF R
.
ЫЫR S
	Refcursor
ЫЫS \
)
ЫЫ\ ]
{
ЬЬ 
	Direction
ЭЭ 
=
ЭЭ  
ParameterDirection
ЭЭ  2
.
ЭЭ2 3
InputOutput
ЭЭ3 >
,
ЭЭ> ?
Value
ЮЮ 
=
ЮЮ 
$str
ЮЮ 1
}
ЯЯ 
;
ЯЯ 
command
°° 
.
°° 
AddParameter
°° $
(
°°$ %
cursorParameter
°°% 4
)
°°4 5
;
°°5 6
await
££ 
command
££ 
.
££ "
ExecuteNonQueryAsync
££ 2
(
££2 3
cancellationToken
££3 D
)
££D E
;
££E F
var
•• 

cursorName
•• 
=
••  
Convert
••! (
.
••( )
ToString
••) 1
(
••1 2
cursorParameter
••2 A
.
••A B
Value
••B G
)
••G H
??
••I K
$str
••L a
;
••a b
await
ІІ 
using
ІІ 
var
ІІ 
fetchCommand
ІІ  ,
=
ІІ- .

connection
ІІ/ 9
.
ІІ9 :
CreateCommand
ІІ: G
(
ІІG H
$"
ІІH J
$str
ІІJ Y
{
ІІY Z

cursorName
ІІZ d
}
ІІd e
"
ІІe f
,
ІІf g
transaction
ІІh s
)
ІІs t
;
ІІt u
await
®® 
using
®® 
var
®® 
reader
®®  &
=
®®' (
await
®®) .
fetchCommand
®®/ ;
.
®®; < 
ExecuteReaderAsync
®®< N
(
®®N O
cancellationToken
®®O `
)
®®` a
;
®®a b
while
™™ 
(
™™ 
await
™™ 
reader
™™ #
.
™™# $
	ReadAsync
™™$ -
(
™™- .
cancellationToken
™™. ?
)
™™? @
)
™™@ A
{
ЂЂ 
clientes
ђђ 
.
ђђ 
Add
ђђ  
(
ђђ  !

MapCliente
ђђ! +
(
ђђ+ ,
reader
ђђ, 2
)
ђђ2 3
)
ђђ3 4
;
ђђ4 5
}
≠≠ 
}
ЃЃ 
await
∞∞ 
transaction
∞∞ 
.
∞∞ 
CommitAsync
∞∞ )
(
∞∞) *
cancellationToken
∞∞* ;
)
∞∞; <
;
∞∞< =
return
≤≤ 
clientes
≤≤ 
;
≤≤ 
}
≥≥ 	
catch
іі 
(
іі 
	Exception
іі 
ex
іі 
)
іі 
{
µµ 	
_logger
ґґ 
.
ґґ 
LogError
ґґ 
(
ґґ 
ex
ґґ 
,
ґґ  
$str
ґґ! O
,
ґґO P
search
ґґQ W
)
ґґW X
;
ґґX Y
throw
ЈЈ 
;
ЈЈ 
}
ЄЄ 	
}
єє 
private
її 
static
її 
Cliente
її 

MapCliente
її %
(
її% &'
NpgsqlParameterCollection
її& ?

parameters
її@ J
)
їїJ K
{
ЉЉ 
return
љљ 
new
љљ 
Cliente
љљ 
{
ЊЊ 	
Id
њњ 
=
њњ 
Convert
њњ 
.
њњ 
ToInt32
њњ  
(
њњ  !

parameters
њњ! +
[
њњ+ ,
$str
њњ, 2
]
њњ2 3
.
њњ3 4
Value
њњ4 9
)
њњ9 :
,
њњ: ;
Nombre
јј 
=
јј 
Convert
јј 
.
јј 
ToString
јј %
(
јј% &

parameters
јј& 0
[
јј0 1
$str
јј1 ;
]
јј; <
.
јј< =
Value
јј= B
)
јјB C
??
јјD F
string
јјG M
.
јјM N
Empty
јјN S
,
јјS T
Apellido
ЅЅ 
=
ЅЅ 
Convert
ЅЅ 
.
ЅЅ 
ToString
ЅЅ '
(
ЅЅ' (

parameters
ЅЅ( 2
[
ЅЅ2 3
$str
ЅЅ3 ?
]
ЅЅ? @
.
ЅЅ@ A
Value
ЅЅA F
)
ЅЅF G
??
ЅЅH J
string
ЅЅK Q
.
ЅЅQ R
Empty
ЅЅR W
,
ЅЅW X
RazonSocial
¬¬ 
=
¬¬ 
Convert
¬¬ !
.
¬¬! "
ToString
¬¬" *
(
¬¬* +

parameters
¬¬+ 5
[
¬¬5 6
$str
¬¬6 F
]
¬¬F G
.
¬¬G H
Value
¬¬H M
)
¬¬M N
??
¬¬O Q
string
¬¬R X
.
¬¬X Y
Empty
¬¬Y ^
,
¬¬^ _
Cuit
√√ 
=
√√ 
Convert
√√ 
.
√√ 
ToString
√√ #
(
√√# $

parameters
√√$ .
[
√√. /
$str
√√/ 7
]
√√7 8
.
√√8 9
Value
√√9 >
)
√√> ?
??
√√@ B
string
√√C I
.
√√I J
Empty
√√J O
,
√√O P
FechaNacimiento
ƒƒ 
=
ƒƒ 

ToDateOnly
ƒƒ (
(
ƒƒ( )

parameters
ƒƒ) 3
[
ƒƒ3 4
$str
ƒƒ4 H
]
ƒƒH I
.
ƒƒI J
Value
ƒƒJ O
)
ƒƒO P
,
ƒƒP Q
TelefonoCelular
≈≈ 
=
≈≈ 
Convert
≈≈ %
.
≈≈% &
ToString
≈≈& .
(
≈≈. /

parameters
≈≈/ 9
[
≈≈9 :
$str
≈≈: N
]
≈≈N O
.
≈≈O P
Value
≈≈P U
)
≈≈U V
??
≈≈W Y
string
≈≈Z `
.
≈≈` a
Empty
≈≈a f
,
≈≈f g
Email
∆∆ 
=
∆∆ 
Convert
∆∆ 
.
∆∆ 
ToString
∆∆ $
(
∆∆$ %

parameters
∆∆% /
[
∆∆/ 0
$str
∆∆0 9
]
∆∆9 :
.
∆∆: ;
Value
∆∆; @
)
∆∆@ A
??
∆∆B D
string
∆∆E K
.
∆∆K L
Empty
∆∆L Q
,
∆∆Q R
FechaCreacion
«« 
=
«« 
Convert
«« #
.
««# $

ToDateTime
««$ .
(
««. /

parameters
««/ 9
[
««9 :
$str
««: L
]
««L M
.
««M N
Value
««N S
)
««S T
,
««T U
FechaModificacion
»» 
=
»» 
Convert
»»  '
.
»»' (

ToDateTime
»»( 2
(
»»2 3

parameters
»»3 =
[
»»= >
$str
»»> T
]
»»T U
.
»»U V
Value
»»V [
)
»»[ \
}
…… 	
;
……	 

}
   
private
ћћ 
static
ћћ 
Cliente
ћћ 

MapCliente
ћћ %
(
ћћ% &
DbDataReader
ћћ& 2
reader
ћћ3 9
)
ћћ9 :
{
ЌЌ 
return
ќќ 
new
ќќ 
Cliente
ќќ 
{
ѕѕ 	
Id
–– 
=
–– 
reader
–– 
.
–– 
GetInt32
––  
(
––  !
reader
––! '
.
––' (

GetOrdinal
––( 2
(
––2 3
$str
––3 7
)
––7 8
)
––8 9
,
––9 :
Nombre
—— 
=
—— 
reader
—— 
.
—— 
	GetString
—— %
(
——% &
reader
——& ,
.
——, -

GetOrdinal
——- 7
(
——7 8
$str
——8 @
)
——@ A
)
——A B
,
——B C
Apellido
““ 
=
““ 
reader
““ 
.
““ 
	GetString
““ '
(
““' (
reader
““( .
.
““. /

GetOrdinal
““/ 9
(
““9 :
$str
““: D
)
““D E
)
““E F
,
““F G
RazonSocial
”” 
=
”” 
reader
””  
.
””  !
	GetString
””! *
(
””* +
reader
””+ 1
.
””1 2

GetOrdinal
””2 <
(
””< =
$str
””= K
)
””K L
)
””L M
,
””M N
Cuit
‘‘ 
=
‘‘ 
reader
‘‘ 
.
‘‘ 
	GetString
‘‘ #
(
‘‘# $
reader
‘‘$ *
.
‘‘* +

GetOrdinal
‘‘+ 5
(
‘‘5 6
$str
‘‘6 <
)
‘‘< =
)
‘‘= >
,
‘‘> ?
FechaNacimiento
’’ 
=
’’ 
reader
’’ $
.
’’$ %
GetFieldValue
’’% 2
<
’’2 3
DateOnly
’’3 ;
>
’’; <
(
’’< =
reader
’’= C
.
’’C D

GetOrdinal
’’D N
(
’’N O
$str
’’O a
)
’’a b
)
’’b c
,
’’c d
TelefonoCelular
÷÷ 
=
÷÷ 
reader
÷÷ $
.
÷÷$ %
	GetString
÷÷% .
(
÷÷. /
reader
÷÷/ 5
.
÷÷5 6

GetOrdinal
÷÷6 @
(
÷÷@ A
$str
÷÷A S
)
÷÷S T
)
÷÷T U
,
÷÷U V
Email
„„ 
=
„„ 
reader
„„ 
.
„„ 
	GetString
„„ $
(
„„$ %
reader
„„% +
.
„„+ ,

GetOrdinal
„„, 6
(
„„6 7
$str
„„7 >
)
„„> ?
)
„„? @
,
„„@ A
FechaCreacion
ЎЎ 
=
ЎЎ 
reader
ЎЎ "
.
ЎЎ" #
GetDateTime
ЎЎ# .
(
ЎЎ. /
reader
ЎЎ/ 5
.
ЎЎ5 6

GetOrdinal
ЎЎ6 @
(
ЎЎ@ A
$str
ЎЎA Q
)
ЎЎQ R
)
ЎЎR S
,
ЎЎS T
FechaModificacion
ўў 
=
ўў 
reader
ўў  &
.
ўў& '
GetDateTime
ўў' 2
(
ўў2 3
reader
ўў3 9
.
ўў9 :

GetOrdinal
ўў: D
(
ўўD E
$str
ўўE Y
)
ўўY Z
)
ўўZ [
}
ЏЏ 	
;
ЏЏ	 

}
џџ 
private
ЁЁ 
static
ЁЁ 
DateOnly
ЁЁ 

ToDateOnly
ЁЁ &
(
ЁЁ& '
object
ЁЁ' -
value
ЁЁ. 3
)
ЁЁ3 4
{
ёё 
return
яя 
value
яя 
switch
яя 
{
аа 	
DateOnly
бб 
dateOnly
бб 
=>
бб  
dateOnly
бб! )
,
бб) *
DateTime
вв 
dateTime
вв 
=>
вв  
DateOnly
вв! )
.
вв) *
FromDateTime
вв* 6
(
вв6 7
dateTime
вв7 ?
)
вв? @
,
вв@ A
_
гг 
=>
гг 
DateOnly
гг 
.
гг 
FromDateTime
гг &
(
гг& '
Convert
гг' .
.
гг. /

ToDateTime
гг/ 9
(
гг9 :
value
гг: ?
)
гг? @
)
гг@ A
}
дд 	
;
дд	 

}
ее 
}жж ±
ГC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Infrastructure\Database\INpgsqlTransactionWrapper.cs
	namespace 	
Clientes
 
. 
Infrastructure !
.! "
Database" *
;* +
public 
	interface %
INpgsqlTransactionWrapper *
:+ ,
IAsyncDisposable- =
{ 
Task 
CommitAsync	 
( 
CancellationToken &
cancellationToken' 8
)8 9
;9 :
} 
internal

 
class

	 $
NpgsqlTransactionWrapper

 '
:

( )%
INpgsqlTransactionWrapper

* C
{ 
private 
readonly 
NpgsqlTransaction &
_transaction' 3
;3 4
public 
$
NpgsqlTransactionWrapper #
(# $
NpgsqlTransaction$ 5
transaction6 A
)A B
{ 
_transaction 
= 
transaction "
;" #
} 
public 

NpgsqlTransaction 
Transaction (
=>) +
_transaction, 8
;8 9
public 

Task 
CommitAsync 
( 
CancellationToken -
cancellationToken. ?
)? @
{ 
return 
_transaction 
. 
CommitAsync '
(' (
cancellationToken( 9
)9 :
;: ;
} 
public 

	ValueTask 
DisposeAsync !
(! "
)" #
{ 
return 
_transaction 
. 
DisposeAsync (
(( )
)) *
;* +
} 
} ў
ВC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Infrastructure\Database\INpgsqlConnectionWrapper.cs
	namespace 	
Clientes
 
. 
Infrastructure !
.! "
Database" *
;* +
public 
	interface $
INpgsqlConnectionWrapper )
:* +
IAsyncDisposable, <
{ 
Task 
	OpenAsync	 
( 
CancellationToken $
cancellationToken% 6
)6 7
;7 8
Task		 
<		 	%
INpgsqlTransactionWrapper			 "
>		" #!
BeginTransactionAsync		$ 9
(		9 :
CancellationToken		: K
cancellationToken		L ]
)		] ^
;		^ _!
INpgsqlCommandWrapper

 
CreateCommand

 '
(

' (
string

( .
commandText

/ :
,

: ;%
INpgsqlTransactionWrapper

< U
?

U V
transaction

W b
=

c d
null

e i
)

i j
;

j k
} 
internal 
class	 #
NpgsqlConnectionWrapper &
:' ($
INpgsqlConnectionWrapper) A
{ 
private 
readonly 
NpgsqlConnection %
_connection& 1
;1 2
public 
#
NpgsqlConnectionWrapper "
(" #
NpgsqlConnection# 3

connection4 >
)> ?
{ 
_connection 
= 

connection  
;  !
} 
public 

Task 
	OpenAsync 
( 
CancellationToken +
cancellationToken, =
)= >
{ 
return 
_connection 
. 
	OpenAsync $
($ %
cancellationToken% 6
)6 7
;7 8
} 
public 

async 
Task 
< %
INpgsqlTransactionWrapper /
>/ 0!
BeginTransactionAsync1 F
(F G
CancellationTokenG X
cancellationTokenY j
)j k
{ 
var 
transaction 
= 
await 
_connection  +
.+ ,!
BeginTransactionAsync, A
(A B
cancellationTokenB S
)S T
.T U
ConfigureAwaitU c
(c d
falsed i
)i j
;j k
return 
new $
NpgsqlTransactionWrapper +
(+ ,
transaction, 7
)7 8
;8 9
} 
public!! 
!
INpgsqlCommandWrapper!!  
CreateCommand!!! .
(!!. /
string!!/ 5
commandText!!6 A
,!!A B%
INpgsqlTransactionWrapper!!C \
?!!\ ]
transaction!!^ i
=!!j k
null!!l p
)!!p q
{"" 
var## 
npgsqlTransaction## 
=## 
(##  !
transaction##! ,
as##- /$
NpgsqlTransactionWrapper##0 H
)##H I
?##I J
.##J K
Transaction##K V
;##V W
return$$ 
new$$  
NpgsqlCommandWrapper$$ '
($$' (
new$$( +
NpgsqlCommand$$, 9
($$9 :
commandText$$: E
,$$E F
_connection$$G R
,$$R S
npgsqlTransaction$$T e
)$$e f
)$$f g
;$$g h
}%% 
public'' 

	ValueTask'' 
DisposeAsync'' !
(''! "
)''" #
{(( 
return)) 
_connection)) 
.)) 
DisposeAsync)) '
())' (
)))( )
;))) *
}** 
}++ №
ВC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Infrastructure\Database\INpgsqlConnectionFactory.cs
	namespace 	
Clientes
 
. 
Infrastructure !
.! "
Database" *
;* +
public 
	interface $
INpgsqlConnectionFactory )
{ $
INpgsqlConnectionWrapper 
CreateConnection -
(- .
string. 4
connectionString5 E
)E F
;F G
} 
public

 
class

 #
NpgsqlConnectionFactory

 $
:

% &$
INpgsqlConnectionFactory

' ?
{ 
public 
$
INpgsqlConnectionWrapper #
CreateConnection$ 4
(4 5
string5 ;
connectionString< L
)L M
{ 
return 
new #
NpgsqlConnectionWrapper *
(* +
new+ .
NpgsqlConnection/ ?
(? @
connectionString@ P
)P Q
)Q R
;R S
} 
} Ь"
C:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Infrastructure\Database\INpgsqlCommandWrapper.cs
	namespace 	
Clientes
 
. 
Infrastructure !
.! "
Database" *
;* +
public 
	interface !
INpgsqlCommandWrapper &
:' (
IAsyncDisposable) 9
{		 
CommandType

 
CommandType

 
{

 
get

 !
;

! "
set

# &
;

& '
}

( )%
NpgsqlParameterCollection 

Parameters (
{) *
get+ .
;. /
}0 1
void 
AddParameter	 
( 
NpgsqlParameter %
	parameter& /
)/ 0
;0 1
void 
AddParameters	 
( 
IEnumerable "
<" #
NpgsqlParameter# 2
>2 3

parameters4 >
)> ?
;? @
Task 
< 	
int	 
>  
ExecuteNonQueryAsync "
(" #
CancellationToken# 4
cancellationToken5 F
)F G
;G H
Task 
< 	
DbDataReader	 
> 
ExecuteReaderAsync )
() *
CancellationToken* ;
cancellationToken< M
)M N
;N O
} 
internal 
class	  
NpgsqlCommandWrapper #
:$ %!
INpgsqlCommandWrapper& ;
{ 
private 
readonly 
NpgsqlCommand "
_command# +
;+ ,
public 
 
NpgsqlCommandWrapper 
(  
NpgsqlCommand  -
command. 5
)5 6
{ 
_command 
= 
command 
; 
} 
public 

CommandType 
CommandType "
{ 
get 
=> 
_command 
. 
CommandType #
;# $
set 
=> 
_command 
. 
CommandType #
=$ %
value& +
;+ ,
} 
public!! 
%
NpgsqlParameterCollection!! $

Parameters!!% /
=>!!0 2
_command!!3 ;
.!!; <

Parameters!!< F
;!!F G
public## 

void## 
AddParameter## 
(## 
NpgsqlParameter## ,
	parameter##- 6
)##6 7
{$$ 
_command%% 
.%% 

Parameters%% 
.%% 
Add%% 
(%%  
	parameter%%  )
)%%) *
;%%* +
}&& 
public(( 

void(( 
AddParameters(( 
((( 
IEnumerable(( )
<(() *
NpgsqlParameter((* 9
>((9 :

parameters((; E
)((E F
{)) 
_command** 
.** 

Parameters** 
.** 
AddRange** $
(**$ %

parameters**% /
.**/ 0
ToArray**0 7
(**7 8
)**8 9
)**9 :
;**: ;
}++ 
public-- 

Task-- 
<-- 
int-- 
>--  
ExecuteNonQueryAsync-- )
(--) *
CancellationToken--* ;
cancellationToken--< M
)--M N
{.. 
return// 
_command// 
.//  
ExecuteNonQueryAsync// ,
(//, -
cancellationToken//- >
)//> ?
;//? @
}00 
public22 

async22 
Task22 
<22 
DbDataReader22 "
>22" #
ExecuteReaderAsync22$ 6
(226 7
CancellationToken227 H
cancellationToken22I Z
)22Z [
{33 
return44 
await44 
_command44 
.44 
ExecuteReaderAsync44 0
(440 1
cancellationToken441 B
)44B C
;44C D
}55 
public77 

	ValueTask77 
DisposeAsync77 !
(77! "
)77" #
{88 
return99 
_command99 
.99 
DisposeAsync99 $
(99$ %
)99% &
;99& '
}:: 
};; ФQ
oC:\Users\LENOVO\Desktop\Challenge Intui\backend\clientes\src\Clientes\Clientes.Infrastructure\DatabaseSeeder.cs
	namespace 	
Clientes
 
. 
Infrastructure !
{ 
public 

static 
class 
DatabaseSeeder &
{		 
public

 
static

 
async

 
Task

  
	SeedAsync

! *
(

* +
IServiceProvider

+ ;
services

< D
,

D E
CancellationToken

F W
cancellationToken

X i
=

j k
default

l s
)

s t
{ 	
var 
loggerFactory 
= 
services  (
.( )

GetService) 3
<3 4
ILoggerFactory4 B
>B C
(C D
)D E
;E F
var 
logger 
= 
loggerFactory &
?& '
.' (
CreateLogger( 4
(4 5
$str5 E
)E F
;F G
var 
configuration 
= 
services  (
.( )

GetService) 3
<3 4
IConfiguration4 B
>B C
(C D
)D E
;E F
if 
( 
configuration 
==  
null! %
)% &
{ 
logger 
? 
. 

LogWarning "
(" #
$str# g
)g h
;h i
return 
; 
} 
var 
connectionString  
=! "
configuration# 0
.0 1
GetConnectionString1 D
(D E
$strE X
)X Y
;Y Z
if 
( 
string 
. 
IsNullOrWhiteSpace )
() *
connectionString* :
): ;
); <
{ 
logger 
? 
. 

LogWarning "
(" #
$str# x
)x y
;y z
return 
; 
} 
try 
{ 
await   
DatabaseInitializer   )
.  ) *
EnsureCreatedAsync  * <
(  < =
connectionString  = M
,  M N
logger  O U
,  U V
cancellationToken  W h
)  h i
;  i j
}!! 
catch"" 
("" 
	Exception"" 
ex"" 
)""  
{## 
logger$$ 
?$$ 
.$$ 
LogError$$  
($$  !
ex$$! #
,$$# $
$str$$% T
)$$T U
;$$U V
throw%% 
;%% 
}&& 
}'' 	
}(( 
internal** 
static** 
class** 
DatabaseInitializer** -
{++ 
private,, 
const,, 
string,, 
CreateTableSql,, +
=,,, -
$str,7. 
;77 
private99 
const99 
string99 
	InsertSql99 &
=99' (
$str9>) q
;>>q r
private@@ 
const@@ 
string@@ $
CreateProcedureInsertSql@@ 5
=@@6 7
$str	@Б8 
;
ББ 
private
ГГ 
const
ГГ 
string
ГГ &
CreateProcedureUpdateSql
ГГ 5
=
ГГ6 7
$str
Г÷8 
;
÷÷ 
private
ЎЎ 
const
ЎЎ 
string
ЎЎ &
CreateProcedureDeleteSql
ЎЎ 5
=
ЎЎ6 7
$str
Ўл8 
;
лл 
private
нн 
const
нн 
string
нн '
CreateProcedureGetByIdSql
нн 6
=
нн7 8
$str
нЬ9 
;
ЬЬ 
private
ЮЮ 
const
ЮЮ 
string
ЮЮ &
CreateProcedureGetAllSql
ЮЮ 5
=
ЮЮ6 7
$str
Юі8 
;
іі 
private
ґґ 
const
ґґ 
string
ґґ ,
CreateProcedureSearchByNameSql
ґґ ;
=
ґґ< =
$str
ґг> 
;
гг 
public
ее 
static
ее 
async
ее 
Task
ее   
EnsureCreatedAsync
ее! 3
(
ее3 4
string
ее4 :
connectionString
ее; K
,
ееK L
ILogger
ееM T
?
ееT U
logger
ееV \
,
ее\ ]
CancellationToken
ее^ o 
cancellationTokenееp Б
)ееБ В
{
жж 	
await
зз 
using
зз 
var
зз 

connection
зз &
=
зз' (
new
зз) ,
NpgsqlConnection
зз- =
(
зз= >
connectionString
зз> N
)
ззN O
;
ззO P
await
ии 

connection
ии 
.
ии 
	OpenAsync
ии &
(
ии& '
cancellationToken
ии' 8
)
ии8 9
;
ии9 :
await
кк 
using
кк 
(
кк 
var
кк 
createCommand
кк *
=
кк+ ,
new
кк- 0
NpgsqlCommand
кк1 >
(
кк> ?
CreateTableSql
кк? M
,
ккM N

connection
ккO Y
)
ккY Z
)
ккZ [
{
лл 
await
мм 
createCommand
мм #
.
мм# $"
ExecuteNonQueryAsync
мм$ 8
(
мм8 9
cancellationToken
мм9 J
)
ммJ K
;
ммK L
}
нн 
await
пп 
using
пп 
(
пп 
var
пп 
createInsertProc
пп -
=
пп. /
new
пп0 3
NpgsqlCommand
пп4 A
(
ппA B&
CreateProcedureInsertSql
ппB Z
,
ппZ [

connection
пп\ f
)
ппf g
)
ппg h
{
рр 
await
сс 
createInsertProc
сс &
.
сс& '"
ExecuteNonQueryAsync
сс' ;
(
сс; <
cancellationToken
сс< M
)
ссM N
;
ссN O
}
тт 
await
фф 
using
фф 
(
фф 
var
фф 
createUpdateProc
фф -
=
фф. /
new
фф0 3
NpgsqlCommand
фф4 A
(
ффA B&
CreateProcedureUpdateSql
ффB Z
,
ффZ [

connection
фф\ f
)
ффf g
)
ффg h
{
хх 
await
цц 
createUpdateProc
цц &
.
цц& '"
ExecuteNonQueryAsync
цц' ;
(
цц; <
cancellationToken
цц< M
)
ццM N
;
ццN O
}
чч 
await
щщ 
using
щщ 
(
щщ 
var
щщ 
createDeleteProc
щщ -
=
щщ. /
new
щщ0 3
NpgsqlCommand
щщ4 A
(
щщA B&
CreateProcedureDeleteSql
щщB Z
,
щщZ [

connection
щщ\ f
)
щщf g
)
щщg h
{
ъъ 
await
ыы 
createDeleteProc
ыы &
.
ыы& '"
ExecuteNonQueryAsync
ыы' ;
(
ыы; <
cancellationToken
ыы< M
)
ыыM N
;
ыыN O
}
ьь 
await
юю 
using
юю 
(
юю 
var
юю 
createGetByIdProc
юю .
=
юю/ 0
new
юю1 4
NpgsqlCommand
юю5 B
(
ююB C'
CreateProcedureGetByIdSql
ююC \
,
юю\ ]

connection
юю^ h
)
ююh i
)
ююi j
{
€€ 
await
АА 
createGetByIdProc
АА '
.
АА' ("
ExecuteNonQueryAsync
АА( <
(
АА< =
cancellationToken
АА= N
)
ААN O
;
ААO P
}
ББ 
await
ГГ 
using
ГГ 
(
ГГ 
var
ГГ 
createGetAllProc
ГГ -
=
ГГ. /
new
ГГ0 3
NpgsqlCommand
ГГ4 A
(
ГГA B&
CreateProcedureGetAllSql
ГГB Z
,
ГГZ [

connection
ГГ\ f
)
ГГf g
)
ГГg h
{
ДД 
await
ЕЕ 
createGetAllProc
ЕЕ &
.
ЕЕ& '"
ExecuteNonQueryAsync
ЕЕ' ;
(
ЕЕ; <
cancellationToken
ЕЕ< M
)
ЕЕM N
;
ЕЕN O
}
ЖЖ 
await
ИИ 
using
ИИ 
(
ИИ 
var
ИИ $
createSearchByNameProc
ИИ 3
=
ИИ4 5
new
ИИ6 9
NpgsqlCommand
ИИ: G
(
ИИG H,
CreateProcedureSearchByNameSql
ИИH f
,
ИИf g

connection
ИИh r
)
ИИr s
)
ИИs t
{
ЙЙ 
await
КК $
createSearchByNameProc
КК ,
.
КК, -"
ExecuteNonQueryAsync
КК- A
(
ККA B
cancellationToken
ККB S
)
ККS T
;
ККT U
}
ЛЛ 
await
НН 
using
НН 
(
НН 
var
НН 
countCommand
НН )
=
НН* +
new
НН, /
NpgsqlCommand
НН0 =
(
НН= >
$str
НН> ]
,
НН] ^

connection
НН_ i
)
ННi j
)
ННj k
{
ОО 
var
ПП 
result
ПП 
=
ПП 
await
ПП "
countCommand
ПП# /
.
ПП/ 0 
ExecuteScalarAsync
ПП0 B
(
ППB C
cancellationToken
ППC T
)
ППT U
;
ППU V
if
РР 
(
РР 
result
РР 
is
РР 
long
РР "
total
РР# (
&&
РР) +
total
РР, 1
>
РР2 3
$num
РР4 5
)
РР5 6
{
СС 
logger
ТТ 
?
ТТ 
.
ТТ 
LogInformation
ТТ *
(
ТТ* +
$str
ТТ+ e
)
ТТe f
;
ТТf g
return
УУ 
;
УУ 
}
ФФ 
}
ХХ 
await
ЧЧ 
using
ЧЧ 
(
ЧЧ 
var
ЧЧ 
insertCommand
ЧЧ *
=
ЧЧ+ ,
new
ЧЧ- 0
NpgsqlCommand
ЧЧ1 >
(
ЧЧ> ?
	InsertSql
ЧЧ? H
,
ЧЧH I

connection
ЧЧJ T
)
ЧЧT U
)
ЧЧU V
{
ШШ 
await
ЩЩ 
insertCommand
ЩЩ #
.
ЩЩ# $"
ExecuteNonQueryAsync
ЩЩ$ 8
(
ЩЩ8 9
cancellationToken
ЩЩ9 J
)
ЩЩJ K
;
ЩЩK L
}
ЪЪ 
logger
ЬЬ 
?
ЬЬ 
.
ЬЬ 
LogInformation
ЬЬ "
(
ЬЬ" #
$str
ЬЬ# M
)
ЬЬM N
;
ЬЬN O
}
ЭЭ 	
}
ЮЮ 
}ЯЯ 