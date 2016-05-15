CC=mcs
PRJ=Quads
SRC=Quadtree.cs ISplitable.cs Square.cs $(PRJ).cs
OUT=$(PRJ).exe
REF=System.Drawing.dll

all:$(OUT)
	
$(PRJ):$(OUT)

$(OUT):
	$(CC) -r:$(REF) $(SRC)

clean:
	rm -f $(OUT)
