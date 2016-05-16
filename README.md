Quads
=====

Computer art based on quadtrees.

The program targets an input image. The input image is split into four
quadrants. Each quadrant is assigned an averaged color based on the colors in
the input image. The quadrant with the largest error is split into its four
children quadrants to refine the image. This process is repeated N times.

Example
-------

Find below an example with the compression of this famous picture of Lena.

![Lena](http://i.imgur.com/pBRlf6p.png)

The next photo has been generated with 10000 splits.

![Lena with 10000 splits](http://i.imgur.com/CKLyhDV.png)

We notice that the most contrasted areas are made with a bigger number of
squares. While the less contrasted are made of bigger and less squares.
