void Dither_float(in int2 screenPos, in int bayerLevel, in float spread, out float Out)
{
	static const int bayer2[2 * 2] = {
        0, 2,
        3, 1
    };

    static const int bayer4[4 * 4] = {
        0, 8, 2, 10,
        12, 4, 14, 6,
        3, 11, 1, 9,
        15, 7, 13, 5
    };

    static const int bayer8[8 * 8] = {
        0, 32, 8, 40, 2, 34, 10, 42,
        48, 16, 56, 24, 50, 18, 58, 26,  
        12, 44,  4, 36, 14, 46,  6, 38, 
        60, 28, 52, 20, 62, 30, 54, 22,  
        3, 35, 11, 43,  1, 33,  9, 41,  
        51, 19, 59, 27, 49, 17, 57, 25, 
        15, 47,  7, 39, 13, 45,  5, 37, 
        63, 31, 55, 23, 61, 29, 53, 21
    };

    float bayerValues[3] = { 0, 0, 0 };

    int x = screenPos.x;
    int y = screenPos.y;

    bayerValues[0] = (bayer2[(x % 2) + (y % 2) * 2]) * (1.0f / 4.0f) - 0.5f;
    bayerValues[1] = (bayer4[(x % 4) + (y % 4) * 4]) * (1.0f / 16.0f) - 0.5f;
    bayerValues[2] = (bayer8[(x % 8) + (y % 8) * 8]) * (1.0f / 64.0f) - 0.5f;

    Out = spread * bayerValues[bayerLevel];
}