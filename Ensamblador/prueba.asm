;Autor: Erik, Brayan y Mauricio
;Analizador Lexico

extern fflush
extern printf
extern scanf
extern stdout

section .bss
	scanner resd 1

segment .text
	global main

main:
;Asignacion a y
	mov eax, 10
	push eax
	pop eax
	mov dword [y], eax
;Termina asignación a y
;Asignacion a z
	mov eax, 2
	push eax
	pop eax
	mov dword [z], eax
;Termina asignación a z
;Asignacion a c
	mov eax, 100
	push eax
	pop eax
	mov dword [c], eax
;Termina asignación a c
	push ebp
	mov ebp, esp
	push msg1
	call printf
	mov esp, ebp
	pop ebp
;Asignacion a altura
	push scanner
	push enterowl
	call scanf
	add esp, 8
	mov eax, [scanner]
	mov dword[altura], eax
;Termina asignación a altura
	push ebp
	mov ebp, esp
	push msg2
	call printf
	mov esp, ebp
	pop ebp
;Asignacion a x
	mov eax, 3
	push eax
	mov eax, [altura]
	push eax
	pop ebx
	pop eax
	add eax, ebx
	push eax
	mov eax, 8
	push eax
	pop ebx
	pop eax
	mul ebx
	push eax
	mov eax, 10
	push eax
	mov eax, 4
	push eax
	pop ebx
	pop eax
	sub eax, ebx
	push eax
	mov eax, 2
	push eax
	pop ebx
	pop eax
	div ebx
	push eax
	pop ebx
	pop eax
	sub eax, ebx
	push eax
	pop eax
	mov dword [x], eax
;Termina asignación a x
;Asignacion a x
	mov eax, [x]
	mov ebx, 1
	sub eax, ebx
	mov dword [x], eax
;Termina asignación a x
;Asignacion a x
	mov eax, [altura]
	push eax
	mov eax, 8
	push eax
	pop ebx
	pop eax
	mul ebx
	push eax
	mov eax, [x]
	pop ebx
	add eax, ebx
	mov dword [x], eax
;Termina asignación a x
;Asignacion a x
	mov eax, [x]
	push eax
	mov eax, 2
	push eax
	pop ebx
	pop eax
	mul ebx
	mov dword [x], eax
;Termina asignación a x
;Asignacion a k
	mov eax, 1
	push eax
	pop eax
	mov dword [k], eax
;Termina asignación a k
; for1
;Asignacion a i
	mov eax, 1
	push eax
	pop eax
	mov dword [i], eax
;Termina asignación a i
_for1:
	mov eax, [k]
	push eax
	mov eax, [altura]
	push eax
	pop ebx
	pop eax
	cmp eax, ebx
	ja _forEnd1
	jmp _forAsigEnd1
_forAsig1:
;Asignacion a k
	mov eax, [k]
	mov ebx, 1
	add eax, ebx
	mov dword [k], eax
;Termina asignación a k
	jmp _for1
_forAsigEnd1:
; for2
;Asignacion a j
	mov eax, 1
	push eax
	pop eax
	mov dword [j], eax
;Termina asignación a j
_for2:
	mov eax, [j]
	push eax
	mov eax, [k]
	push eax
	pop ebx
	pop eax
	cmp eax, ebx
	ja _forEnd2
	jmp _forAsigEnd2
_forAsig2:
;Asignacion a j
	mov eax, [j]
	mov ebx, 1
	add eax, ebx
	mov dword [j], eax
;Termina asignación a j
	jmp _for2
_forAsigEnd2:
; if1
	mov eax, [j]
	push eax
	mov eax, 2
	push eax
	pop ebx
	pop eax
	div ebx
	push edx
	mov eax, 0
	push eax
	pop ebx
	pop eax
	cmp eax, ebx
	jne _if1
	push ebp
	mov ebp, esp
	push msg3
	call printf
	mov esp, ebp
	pop ebp
	jmp _else1
_if1:
	push ebp
	mov ebp, esp
	push msg4
	call printf
	mov esp, ebp
	pop ebp
_else1:
	jmp _forAsig2
_forEnd2:
	push ebp
	mov ebp, esp
	push msg5
	call printf
	mov esp, ebp
	pop ebp
	jmp _forAsig1
_forEnd1:
;Asignacion a i
	mov eax, 0
	push eax
	pop eax
	mov dword [i], eax
;Termina asignación a i
; do1
_do1:
	push ebp
	mov ebp, esp
	push msg6
	call printf
	mov esp, ebp
	pop ebp
;Asignacion a i
	mov eax, [i]
	mov ebx, 1
	add eax, ebx
	mov dword [i], eax
;Termina asignación a i
	mov eax, [i]
	push eax
	mov eax, [altura]
	push eax
	mov eax, 2
	push eax
	pop ebx
	pop eax
	mul ebx
	push eax
	pop ebx
	pop eax
	cmp eax, ebx
	jb _do1
	push ebp
	mov ebp, esp
	push msg7
	call printf
	mov esp, ebp
	pop ebp
; for3
;Asignacion a i
	mov eax, 1
	push eax
	pop eax
	mov dword [i], eax
;Termina asignación a i
_for3:
	mov eax, [i]
	push eax
	mov eax, [altura]
	push eax
	pop ebx
	pop eax
	cmp eax, ebx
	ja _forEnd3
	jmp _forAsigEnd3
_forAsig3:
;Asignacion a i
	mov eax, [i]
	mov ebx, 1
	add eax, ebx
	mov dword [i], eax
;Termina asignación a i
	jmp _for3
_forAsigEnd3:
;Asignacion a j
	mov eax, 1
	push eax
	pop eax
	mov dword [j], eax
;Termina asignación a j
; while1
_while1:
	mov eax, [j]
	push eax
	mov eax, [i]
	push eax
	pop ebx
	pop eax
	cmp eax, ebx
	ja _whileEnd1
	push ebp
	mov ebp, esp
	push msg8
	call printf
	mov esp, ebp
	pop ebp
	push dword [j]
	push entero
	call printf
	add esp, 8
;Asignacion a j
	mov eax, [j]
	mov ebx, 1
	add eax, ebx
	mov dword [j], eax
;Termina asignación a j
	jmp _while1
_whileEnd1:
	push ebp
	mov ebp, esp
	push msg10
	call printf
	mov esp, ebp
	pop ebp
	jmp _forAsig3
_forEnd3:
;Asignacion a i
	mov eax, 0
	push eax
	pop eax
	mov dword [i], eax
;Termina asignación a i
; do2
_do2:
	push ebp
	mov ebp, esp
	push msg11
	call printf
	mov esp, ebp
	pop ebp
;Asignacion a i
	mov eax, [i]
	mov ebx, 1
	add eax, ebx
	mov dword [i], eax
;Termina asignación a i
	mov eax, [i]
	push eax
	mov eax, [altura]
	push eax
	mov eax, 2
	push eax
	pop ebx
	pop eax
	mul ebx
	push eax
	pop ebx
	pop eax
	cmp eax, ebx
	jb _do2
	push ebp
	mov ebp, esp
	push msg12
	call printf
	mov esp, ebp
	pop ebp
	add esp, 4
	mov eax, 1
	xor ebx, ebx
	int 0x80

segment .data
	msg1 db "Valor de altura = ", 0
	msg2 db "", 13, 0
	msg3 db "*", 0
	msg4 db "-", 0
	msg5 db "", 13, 0
	msg6 db "-", 0
	msg7 db "", 13, 0
	msg8 db "", 0
	msg10 db "", 13, 0
	msg11 db "-", 0
	msg12 db "", 13, 0
	vacio db  "", 0
	vaciowl db  "", 13, 0
	caracter db  "%c", 0
	caracterwl db "%c", 13,0
	entero db "%d", 0
	enterowl db "%d", 13, 0
	flotante db "%f", 0
	flotantewl db "%f", 13, 0
	altura dd 0
	i dd 0
	j dd 0
	y dq 0 
	z dq 0 
	c db 0
	x dq 0 
	k dd 0
