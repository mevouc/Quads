CC=mcs
PRJ=Quads
SRC=$(PRJ).cs
OUT=$(PRJ).exe

all:$(OUT)
	
$(PRJ):$(OUT)

$(OUT):
	$(CC) $(SRC)

clean:
	rm -f $(OUT)
