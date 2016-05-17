CC=mcs
PRJ=Quads
SRC=src/*
OUT=$(PRJ).exe
REF=System.Drawing.dll
FLAGS=-debug

all:$(OUT)
	
$(PRJ):$(OUT)

$(OUT):
	$(CC) -r:$(REF) $(SRC) $(FLAGS) -out:$(OUT)

clean:
	rm -f $(OUT)*
