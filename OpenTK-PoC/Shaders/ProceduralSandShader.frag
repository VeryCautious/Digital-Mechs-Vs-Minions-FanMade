﻿#version 460

in vec2 fUv;

out vec4 fragColor;


// https://adrianb.io/2014/08/09/perlinnoise.html
const int p[513] = int[]( 151,160,137,91,90,15,
    131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
    190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
    88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
    77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
    102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
    135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
    5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
    223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
    129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
    251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
    49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
    138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
    151,160,137,91,90,15,
    131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
    190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
    88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
    77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
    102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
    135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
    5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
    223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
    129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
    251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
    49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
    138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
    131
);

// p % 16
const int hash[513] = int[](
    7,0,9,11,10,15,3,13,9,15,0,5,2,9,7,1,12,4,7,14,5,14,8,3,5,0,5,10,7,14,6,4,7,8,
    10,11,0,10,5,14,14,12,11,11,5,3,11,0,9,1,1,8,13,5,8,7,14,4,13,8,11,8,4,15,10,5,
    7,6,11,0,11,6,13,2,14,7,3,15,5,10,12,3,5,6,12,9,12,9,7,14,5,8,4,6,15,6,1,9,15,1,
    1,8,0,9,1,12,4,11,0,9,2,9,8,4,7,2,4,12,15,6,4,4,13,6,13,10,3,0,4,9,2,10,12,11,5,
    10,6,3,6,14,15,2,5,4,15,14,11,3,15,0,10,1,6,13,12,10,15,7,10,5,7,8,8,2,12,10,3,
    6,13,9,5,11,7,11,12,9,1,6,7,13,3,2,12,14,15,1,0,8,2,9,0,8,10,6,1,4,11,2,2,1,14,
    2,0,12,15,3,2,1,1,3,1,11,9,14,15,11,1,0,6,15,5,7,10,13,8,4,12,0,3,9,2,13,15,4,6,
    14,10,12,13,13,14,2,3,13,8,8,3,13,0,3,14,2,7,13,12,4,
    7,0,9,11,10,15,3,13,9,15,0,5,2,9,7,1,12,4,7,14,5,14,8,3,5,0,5,10,7,14,6,4,7,8,
    10,11,0,10,5,14,14,12,11,11,5,3,11,0,9,1,1,8,13,5,8,7,14,4,13,8,11,8,4,15,10,5,
    7,6,11,0,11,6,13,2,14,7,3,15,5,10,12,3,5,6,12,9,12,9,7,14,5,8,4,6,15,6,1,9,15,1,
    1,8,0,9,1,12,4,11,0,9,2,9,8,4,7,2,4,12,15,6,4,4,13,6,13,10,3,0,4,9,2,10,12,11,5,
    10,6,3,6,14,15,2,5,4,15,14,11,3,15,0,10,1,6,13,12,10,15,7,10,5,7,8,8,2,12,10,3,
    6,13,9,5,11,7,11,12,9,1,6,7,13,3,2,12,14,15,1,0,8,2,9,0,8,10,6,1,4,11,2,2,1,14,
    2,0,12,15,3,2,1,1,3,1,11,9,14,15,11,1,0,6,15,5,7,10,13,8,4,12,0,3,9,2,13,15,4,6,
    14,10,12,13,13,14,2,3,13,8,8,3,13,0,3,14,2,7,13,12,4,
    7
);

float grad(int h, float x, float y)
{
    switch(h)
    {
        case 0: return  x + y;
        case 1: return -x + y;
        case 2: return  x - y;
        case 3: return -x - y;
        case 4: return  x + y;
        case 5: return -x + y;
        case 6: return  x - y;
        case 7: return -x - y;
        case 8: return  x + y;
        case 9: return -x + y;
        case 10: return  x - y;
        case 11: return -x - y;
        case 12: return  x + y;
        case 13: return -x + y;
        case 14: return  x - y;
        case 15: return -x - y;
        default: return 0;
    }
}

float fade(float t) {
    return t * t * t * (t * (t * 6 - 15) + 10);
}

float noise(vec2 pos, float scale)
{
    vec2 coo = (pos + vec2(1.0, 1.0)) * scale;

    float flx = floor(coo.x);
    float fly = floor(coo.y);

    int gridX = int(mod(flx, 255));
    int gridY = int(mod(fly, 255));

    float ix = coo.x - flx;
    float iy = coo.y - fly;

    float u = fade(ix);
    float v = fade(iy);

    int aa = hash[p[gridX  ]+ gridY  ];
    int ab = hash[p[gridX  ]+ gridY+1];
    int ba = hash[p[gridX+1]+ gridY  ];
    int bb = hash[p[gridX+1]+ gridY+1];

    float x1 = mix(
        grad(aa, ix  , iy),
        grad(ba, ix-1, iy),
        u);
    float x2 = mix(
        grad(ab, ix  , iy-1),
        grad(bb, ix-1, iy-1),
        u);
    return (mix(x1, x2, v) + 1.0) / 2.0;
}

void main()
{
    float n = noise(fUv, 32);
    vec4 baseColor = vec4(1.0, 0.9, 0.8, 1.0);
    vec4 noiseColor = vec4(0.6, 0.3, 0.0, 1.0);
    fragColor = n * noiseColor + (1-n) * baseColor;
}