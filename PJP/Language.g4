grammar Language;

// Program
program: statement* EOF ;

// Statements
statement
    : ';'
    | declaration
    | expression ';'
    | readStatement
    | writeStatement
    | fopenStatement
    | block
    | ifStatement
    | whileStatement
    ;

// Declarations
declaration: type variableList ';' ;

variableList: IDENTIFIER (',' IDENTIFIER)* ;

// Read/Write
readStatement: 'read' variableList ';' ;
writeStatement: 'write' expressionList ';' ;
fopenStatement: 'fopen' IDENTIFIER expression ';';

// Blocks
block: '{' statement* '}' ;

// If / Else
ifStatement: 'if' '(' expression ')' statement ('else' statement)? ;

// While
whileStatement: 'while' '(' expression ')' statement ;

// Expressions
expression
    : expression '&&' expression                 # andExpr
    | expression '||' expression                 # orExpr
    | expression ('==' | '!=') expression        # eqExpr
    | expression ('<' | '>') expression          # relExpr
    | expression ('*' | '/' | '%') expression    # mulExpr
    | expression ('+' | '-' | '.') expression    # addExpr
    | expression '<<' expression            # fileOutputExpr
    | expression '=' expression                 # assignExpr
    | '!' expression                             # notExpr
    | '-' expression                             # uminusExpr
    | '(' expression ')'                         # parensExpr
    | literal                                    # literalExpr
    | IDENTIFIER                                 # variableExpr
    ;

// Literals
literal
    : INT_LITERAL
    | FLOAT_LITERAL
    | BOOL_LITERAL
    | STRING_LITERAL
    ;

// Expression list
expressionList: expression (',' expression)* ;

// Types
type: 'int' | 'float' | 'bool' | 'string' | 'file' ;

// Tokens
BOOL_LITERAL: 'true' | 'false' ;
IDENTIFIER: [a-zA-Z] [a-zA-Z0-9]* ;
INT_LITERAL: [0-9]+ ;
FLOAT_LITERAL: [0-9]+ '.' [0-9]+ ;
STRING_LITERAL: '"' (~["\\] | '\\' .)* '"' ;

// Ignored
WS: [ \t\r\n]+ -> skip ;
COMMENT: '//' ~[\r\n]* -> skip ;