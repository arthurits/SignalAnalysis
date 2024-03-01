﻿namespace SignalAnalysis.UnitTest;

[TestClass]
public class DerivativeTest
{
    private readonly double[] sin1Hz = [0, 0.0627905195293134, 0.1253332335643040, 0.1873813145857250, 0.2486898871648550, 0.3090169943749470, 0.3681245526846780,
        0.4257792915650730, 0.4817536741017150, 0.5358267949789970, 0.5877852522924730, 0.6374239897486900, 0.6845471059286890, 0.7289686274214110, 0.7705132427757890,
        0.8090169943749470, 0.8443279255020150, 0.8763066800438640, 0.9048270524660200, 0.9297764858882510, 0.9510565162951540, 0.9685831611286310, 0.9822872507286890,
        0.9921147013144780, 0.9980267284282720, 1, 0.9980267284282720, 0.9921147013144780, 0.9822872507286890, 0.9685831611286310, 0.9510565162951530, 0.9297764858882510,
        0.9048270524660190, 0.8763066800438630, 0.8443279255020150, 0.8090169943749470, 0.7705132427757890, 0.7289686274214110, 0.6845471059286880, 0.6374239897486890,
        0.5877852522924720, 0.5358267949789960, 0.4817536741017140, 0.4257792915650720, 0.3681245526846770, 0.3090169943749460, 0.2486898871648540, 0.1873813145857230,
        0.1253332335643030, 0.0627905195293118, 0, -0.0627905195293147, -0.1253332335643060, -0.1873813145857260, -0.2486898871648560, -0.3090169943749490, -0.3681245526846800,
        -0.4257792915650740, -0.4817536741017170, -0.5358267949789980, -0.5877852522924740, -0.6374239897486910, -0.6845471059286900, -0.7289686274214130, -0.7705132427757900,
        -0.8090169943749490, -0.8443279255020160, -0.8763066800438640, -0.9048270524660200, -0.9297764858882520, -0.9510565162951540, -0.9685831611286320, -0.9822872507286890,
        -0.9921147013144780, -0.9980267284282720, -1, -0.9980267284282710, -0.9921147013144780, -0.9822872507286880, -0.9685831611286300, -0.9510565162951530,
        -0.9297764858882500, -0.9048270524660180, -0.8763066800438620, -0.8443279255020140, -0.8090169943749450, -0.7705132427757870, -0.7289686274214090, -0.6845471059286860,
        -0.6374239897486870, -0.5877852522924700, -0.5358267949789930, -0.4817536741017120, -0.4257792915650700, -0.3681245526846750, -0.3090169943749440, -0.2486898871648510,
        -0.1873813145857210, -0.1253332335643000, -0.0627905195293097, 0];

    private double[] Dsin1Hz = [6.283185307, 6.270786876, 6.233640514, 6.171892821, 6.085787487, 5.975664329, 5.841957955, 5.685196042, 5.505997257, 5.305068816,
        5.083203692, 4.841277486, 4.580244969, 4.301136318, 4.005053047, 3.693163661, 3.366699045, 3.026947607, 2.675250189, 2.312994781, 1.941611039, 1.562564645, 1.177351523,
        0.787491932, 0.39452447, 0, -0.39452447, -0.787491932, -1.177351523, -1.562564645, -1.941611039, -2.312994781, -2.675250189, -3.026947607, -3.366699045, -3.693163661,
        -4.005053047, -4.301136318, -4.580244969, -4.841277486, -5.083203692, -5.305068816, -5.505997257, -5.685196042, -5.841957955, -5.975664329, -6.085787487, -6.171892821,
        -6.233640514, -6.270786876, -6.283185307, -6.270786876, -6.233640514, -6.171892821, -6.085787487, -5.975664329, -5.841957955, -5.685196042, -5.505997257, -5.305068816,
        -5.083203692, -4.841277486, -4.580244969, -4.301136318, -4.005053047, -3.693163661, -3.366699045, -3.026947607, -2.675250189, -2.312994781, -1.941611039, -1.562564645,
        -1.177351523, -0.787491932, -0.39452447, 0, 0.39452447, 0.787491932, 1.177351523, 1.562564645, 1.941611039, 2.312994781, 2.675250189, 3.026947607, 3.366699045,
        3.693163661, 4.005053047, 4.301136318, 4.580244969, 4.841277486, 5.083203692, 5.305068816, 5.505997257, 5.685196042, 5.841957955, 5.975664329, 6.085787487, 6.171892821,
        6.233640514, 6.270786876, 6.283185307];

    private readonly double[] sin2Hz = [0, 0.0626666167821521, 0.1243449435824270, 0.1840622763423390, 0.2408768370508580, 0.2938926261462370, 0.3422735529643440,
        0.3852566213878950, 0.4221639627510080, 0.4524135262330100, 0.4755282581475770, 0.4911436253643440, 0.4990133642141360, 0.4990133642141360, 0.4911436253643440,
        0.4755282581475770, 0.4524135262330100, 0.4221639627510070, 0.3852566213878950, 0.3422735529643440, 0.2938926261462360, 0.2408768370508570, 0.1840622763423390,
        0.1243449435824270, 0.0626666167821516, 0, -0.0626666167821524, -0.1243449435824280, -0.1840622763423390, -0.2408768370508580, -0.2938926261462370, -0.3422735529643450,
        -0.3852566213878950, -0.4221639627510080, -0.4524135262330100, -0.4755282581475770, -0.4911436253643450, -0.4990133642141360, -0.4990133642141360, -0.4911436253643440,
        -0.4755282581475760, -0.4524135262330090, -0.4221639627510070, -0.3852566213878940, -0.3422735529643440, -0.2938926261462360, -0.2408768370508560, -0.1840622763423380,
        -0.1243449435824260, -0.0626666167821506, 0, 0.0626666167821534, 0.1243449435824290, 0.1840622763423400, 0.2408768370508590, 0.2938926261462380, 0.3422735529643460,
        0.3852566213878960, 0.4221639627510090, 0.4524135262330100, 0.4755282581475770, 0.4911436253643450, 0.4990133642141360, 0.4990133642141360, 0.4911436253643440,
        0.4755282581475760, 0.4524135262330090, 0.4221639627510070, 0.3852566213878930, 0.3422735529643430, 0.2938926261462350, 0.2408768370508560, 0.1840622763423360,
        0.1243449435824250, 0.0626666167821493, 0, -0.0626666167821551, -0.1243449435824300, -0.1840622763423420, -0.2408768370508600, -0.2938926261462390, -0.3422735529643470,
        -0.3852566213878960, -0.4221639627510090, -0.4524135262330110, -0.4755282581475780, -0.4911436253643450, -0.4990133642141360, -0.4990133642141360, -0.4911436253643440,
        -0.4755282581475760, -0.4524135262330080, -0.4221639627510060, -0.3852566213878930, -0.3422735529643420, -0.2938926261462340, -0.2408768370508540, -0.1840622763423360,
        -0.1243449435824230, -0.0626666167821485, 0];

    private double[] Dsin2Hz = [6.283185307, 6.233640514, 6.085787487, 5.841957955, 5.505997257, 5.083203692, 4.580244969, 4.005053047, 3.366699045, 2.675250189,
        1.941611039, 1.177351523, 0.39452447, -0.39452447, -1.177351523, -1.941611039, -2.675250189, -3.366699045, -4.005053047, -4.580244969, -5.083203692, -5.505997257,
        -5.841957955, -6.085787487, -6.233640514, -6.283185307, -6.233640514, -6.085787487, -5.841957955, -5.505997257, -5.083203692, -4.580244969, -4.005053047, -3.366699045,
        -2.675250189, -1.941611039, -1.177351523, -0.39452447, 0.39452447, 1.177351523, 1.941611039, 2.675250189, 3.366699045, 4.005053047, 4.580244969, 5.083203692,
        5.505997257, 5.841957955, 6.085787487, 6.233640514, 6.283185307, 6.233640514, 6.085787487, 5.841957955, 5.505997257, 5.083203692, 4.580244969, 4.005053047, 3.366699045,
        2.675250189, 1.941611039, 1.177351523, 0.39452447, -0.39452447, -1.177351523, -1.941611039, -2.675250189, -3.366699045, -4.005053047, -4.580244969, -5.083203692,
        -5.505997257, -5.841957955, -6.085787487, -6.233640514, -6.283185307, -6.233640514, -6.085787487, -5.841957955, -5.505997257, -5.083203692, -4.580244969, -4.005053047,
        -3.366699045, -2.675250189, -1.941611039, -1.177351523, -0.39452447, 0.39452447, 1.177351523, 1.941611039, 2.675250189, 3.366699045, 4.005053047, 4.580244969,
        5.083203692, 5.505997257, 5.841957955, 6.085787487, 6.233640514, 6.283185307];

    private readonly double[] sinSum = [0, 0.125457136311465, 0.249678177146732, 0.371443590928064, 0.489566724215712, 0.602909620521184, 0.710398105649022,
        0.811035912952967, 0.903917636852723, 0.988240321212006, 1.063313510440050, 1.128567615113030, 1.183560470142820, 1.227981991635550, 1.261656868140130,
        1.284545252522520, 1.296741451735020, 1.298470642794870, 1.290083673853910, 1.272050038852600, 1.244949142441390, 1.209459998179490, 1.166349527071030,
        1.116459644896900, 1.060693345210420, 1, 0.935360111646119, 0.867769757732050, 0.798224974386349, 0.727706324077773, 0.657163890148916, 0.587502932923906,
        0.519570431078124, 0.454142717292855, 0.391914399269005, 0.333488736227370, 0.279369617411444, 0.229955263207275, 0.185533741714552, 0.146280364384345,
        0.112256994144896, 0.083413268745986, 0.059589711350707, 0.040522670177178, 0.025850999720333, 0.015124368228711, 0.007813050113997, 0.003319038243386,
        0.000988289981877, 0.000123902747161, 0, -0.000123902747161, -0.000988289981877, -0.003319038243386, -0.007813050113997, -0.015124368228711, -0.025850999720334,
        -0.040522670177179, -0.059589711350708, -0.083413268745988, -0.112256994144897, -0.146280364384346, -0.185533741714554, -0.229955263207277, -0.279369617411447,
        -0.333488736227373, -0.391914399269007, -0.454142717292858, -0.519570431078127, -0.587502932923909, -0.657163890148920, -0.727706324077776, -0.798224974386353,
        -0.867769757732053, -0.935360111646122, -1, -1.060693345210430, -1.116459644896910, -1.166349527071030, -1.209459998179490, -1.244949142441390, -1.272050038852600,
        -1.290083673853910, -1.298470642794870, -1.296741451735020, -1.284545252522520, -1.261656868140130, -1.227981991635550, -1.183560470142820, -1.128567615113030,
        -1.063313510440050, -0.988240321212001, -0.903917636852718, -0.811035912952963, -0.710398105649016, -0.602909620521178, -0.489566724215705, -0.371443590928057,
        -0.249678177146724, -0.125457136311458, 0];

    private double[] DsinSum = [12.56637061, 12.50442739, 12.319428, 12.01385078, 11.59178474, 11.05886802, 10.42220292, 9.690249088, 8.872696302, 7.980319005,
        7.024814731, 6.018629009, 4.974769439, 3.906611848, 2.827701524, 1.751552622, 0.691448857, -0.339751439, -1.329802858, -2.267250189, -3.141592654, -3.943432612,
        -4.664606432, -5.298295555, -5.839116045, -6.283185307, -6.628164984, -6.873279418, -7.019309478, -7.068561902, -7.024814731, -6.89323975, -6.680303236, -6.393646652,
        -6.041949234, -5.6347747, -5.182404569, -4.695660788, -4.185720499, -3.663925963, -3.141592654, -2.629818627, -2.139298211, -1.680142995, -1.261712986, -0.892460637,
        -0.57979023, -0.329934866, -0.147853028, -0.037146362, 0, -0.037146362, -0.147853028, -0.329934866, -0.57979023, -0.892460637, -1.261712986, -1.680142995, -2.139298211,
        -2.629818627, -3.141592654, -3.663925963, -4.185720499, -4.695660788, -5.182404569, -5.6347747, -6.041949234, -6.393646652, -6.680303236, -6.89323975, -7.024814731,
        -7.068561902, -7.019309478, -6.873279418, -6.628164984, -6.283185307, -5.839116045, -5.298295555, -4.664606432, -3.943432612, -3.141592654, -2.267250189, -1.329802858,
        -0.339751439, 0.691448857, 1.751552622, 2.827701524, 3.906611848, 4.974769439, 6.018629009, 7.024814731, 7.980319005, 8.872696302, 9.690249088, 10.42220292, 11.05886802,
        11.59178474, 12.01385078, 12.319428, 12.50442739, 12.56637061];

    [TestMethod]
    public void Test_Derivative_BackwardOnePoint()
    {
        Dsin1Hz = [double.NaN, 6.279051952931340, 6.254271403499090, 6.204808102142040, 6.130857257913020, 6.032710721009260, 5.910755830973060, 5.765473888039470,
            5.597438253664260, 5.407312087728140, 5.195845731347650, 4.963873745621650, 4.712311617999890, 4.442152149272280, 4.154461535437780, 3.850375159915830,
            3.531093112706760, 3.197875454184860, 2.852037242215590, 2.494943342223190, 2.128003040690220, 1.752664483347750, 1.370408960005750, 0.982745058578915,
            0.591202711379368, 0.197327157172844, -0.197327157172844, -0.591202711379379, -0.982745058578915, -1.370408960005760, -1.752664483347750, -2.128003040690230,
            -2.494943342223190, -2.852037242215600, -3.197875454184850, -3.531093112706770, -3.850375159915830, -4.154461535437780, -4.442152149272290, -4.712311617999910,
            -4.963873745621670, -5.195845731347650, -5.407312087728140, -5.597438253664230, -5.765473888039470, -5.910755830973060, -6.032710721009270, -6.130857257913020,
            -6.204808102142040, -6.254271403499090, -6.279051952931330, -6.279051952931340, -6.254271403499090, -6.204808102142040, -6.130857257913020, -6.032710721009270,
            -5.910755830973050, -5.765473888039460, -5.597438253664260, -5.407312087728090, -5.195845731347650, -4.963873745621660, -4.712311617999880, -4.442152149272280,
            -4.154461535437770, -3.850375159915840, -3.531093112706730, -3.197875454184830, -2.852037242215600, -2.494943342223160, -2.128003040690220, -1.752664483347730,
            -1.370408960005750, -0.982745058578892, -0.591202711379368, -0.197327157172822, 0.197327157172866, 0.591202711379379, 0.982745058578948, 1.370408960005760,
            1.752664483347760, 2.128003040690240, 2.494943342223200, 2.852037242215630, 3.197875454184850, 3.531093112706800, 3.850375159915810, 4.154461535437820,
            4.442152149272270, 4.712311617999950, 4.963873745621630, 5.195845731347710, 5.407312087728110, 5.597438253664230, 5.765473888039520, 5.910755830973020,
            6.032710721009320, 6.130857257912980, 6.204808102142090, 6.254271403499050, 6.279051952931390];

        Dsin2Hz = [double.NaN, 6.266661678215210, 6.167832680027530, 5.971733275991160, 5.681456070851870, 5.301578909537890, 4.838092681810780, 4.298306842355030,
            3.690734136311290, 3.024956348200230, 2.311473191456700, 1.561536721676750, 0.786973884979148, 0, -0.786973884979141, -1.561536721676750, -2.311473191456710,
            -3.024956348200220, -3.690734136311280, -4.298306842355030, -4.838092681810780, -5.301578909537890, -5.681456070851870, -5.971733275991160, -6.167832680027530,
            -6.266661678215210, -6.266661678215190, -6.167832680027530, -5.971733275991160, -5.681456070851870, -5.301578909537890, -4.838092681810770, -4.298306842355010,
            -3.690734136311290, -3.024956348200220, -2.311473191456700, -1.561536721676750, -0.786973884979130, 0, 0.786973884979158, 1.561536721676770, 2.311473191456720,
            3.024956348200240, 3.690734136311280, 4.298306842355040, 4.838092681810790, 5.301578909537900, 5.681456070851880, 5.971733275991170, 6.167832680027530,
            6.266661678215210, 6.266661678215210, 6.167832680027530, 5.971733275991160, 5.681456070851860, 5.301578909537890, 4.838092681810760, 4.298306842355020,
            3.690734136311270, 3.024956348200180, 2.311473191456680, 1.561536721676740, 0.786973884979125, -0, -0.786973884979169, -1.561536721676800, -2.311473191456710,
            -3.024956348200220, -3.690734136311340, -4.298306842355020, -4.838092681810830, -5.301578909537870, -5.681456070851920, -5.971733275991130, -6.167832680027580,
            -6.266661678215170, -6.266661678215260, -6.167832680027480, -5.971733275991190, -5.681456070851810, -5.301578909537840, -4.838092681810790, -4.298306842354970,
            -3.690734136311290, -3.024956348200170, -2.311473191456680, -1.561536721676710, -0.786973884979108, 0, 0.786973884979191, 1.561536721676790, 2.311473191456760,
            3.024956348200240, 3.690734136311300, 4.298306842355090, 4.838092681810770, 5.301578909537960, 5.681456070851850, 5.971733275991220, 6.167832680027500,
            6.266661678215260];
        
        DsinSum = [double.NaN, 12.5457136311465000, 12.4221040835266000, 12.1765413781332000, 11.8123133287649000, 11.3342896305471000, 10.7488485127838000, 10.0637807303945000,
            9.2881723899755500, 8.4322684359283600, 7.5073189228043600, 6.5254104672984000, 5.4992855029790300, 4.4421521492723100, 3.3674876504586200, 2.2888384382390800,
            1.2196199212500600, 0.1729191059846260, -0.8386968940957020, -1.8033635001318400, -2.7100896411205600, -3.5489144261901400, -4.3110471108461300, -4.9889882174122400,
            -5.5766299686481600, -6.0693345210423700, -6.4639888353880400, -6.7590353914069100, -6.9544783345700700, -7.0518650308576200, -7.0542433928856600, -6.9660957225009900,
            -6.7932501845782000, -6.5427713785268800, -6.2228318023850700, -5.8425663041634700, -5.4119118815925700, -4.9414354204169100, -4.4421521492722800, -3.9253377330207500,
            -3.4023370239448900, -2.8843725398909400, -2.3823557395279100, -1.9067041173529500, -1.4671670456844400, -1.0726631491622700, -0.7311318114713690, -0.4494011870611430,
            -0.2330748261508750, -0.0864387234715588, -0.0123902747161245, -0.0123902747161258, -0.0864387234715629, -0.2330748261508830, -0.4494011870611570, -0.7311318114713800,
            -1.0726631491622900, -1.4671670456844500, -1.9067041173529900, -2.3823557395279100, -2.8843725398909700, -3.4023370239449200, -3.9253377330207600, -4.4421521492723000,
            -4.9414354204169400, -5.4119118815926400, -5.8425663041634400, -6.2228318023850500, -6.5427713785269300, -6.7932501845781900, -6.9660957225010500, -7.0542433928856100,
            -7.0518650308576700, -6.9544783345700200, -6.7590353914069500, -6.4639888353880000, -6.0693345210423900, -5.5766299686481100, -4.9889882174122400, -4.3110471108460400,
            -3.5489144261900700, -2.7100896411205600, -1.8033635001317700, -0.8386968940956800, 0.1729191059846920, 1.2196199212501000, 2.2888384382391000, 3.3674876504587300,
            4.4421521492723000, 5.4992855029791300, 6.5254104672984400, 7.5073189228044700, 8.4322684359283500, 9.2881723899755300, 10.0637807303946000, 10.7488485127838000,
            11.3342896305473000, 11.8123133287648000, 12.1765413781333000, 12.4221040835265000, 12.5457136311466000];

        var result = Derivative.Derivate(sin1Hz, DerivativeMethod.BackwardOnePoint, 0, sin1Hz.GetUpperBound(0), 100);
        for (int i = 1; i < Dsin1Hz.Length; i++)
            Assert.AreEqual(Dsin1Hz[i], result[i], 1e-12);

        result = Derivative.Derivate(sin2Hz, DerivativeMethod.BackwardOnePoint, 0, sin1Hz.GetUpperBound(0), 100);
        for (int i = 1; i < Dsin2Hz.Length; i++)
            Assert.AreEqual(Dsin2Hz[i], result[i], 1e-12);

        result = Derivative.Derivate(sinSum, DerivativeMethod.BackwardOnePoint, 0, sinSum.GetUpperBound(0), 100);
        for (int i = 1; i < DsinSum.Length; i++)
            Assert.AreEqual(DsinSum[i], result[i], 1e-12);
        

        result = Derivative.Derivate((x) => Math.Sin(2 * Math.PI * 1 * x), DerivativeMethod.BackwardOnePoint, 0, 1, 100);
        for (int i = 1; i < Dsin1Hz.Length; i++)
            Assert.AreEqual(Dsin1Hz[i], result[i], 1e-12);

        result = Derivative.Derivate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), DerivativeMethod.BackwardOnePoint, 0, 1, 100);
        for (int i = 1; i < Dsin2Hz.Length; i++)
            Assert.AreEqual(Dsin2Hz[i], result[i], 1e-12);

        result = Derivative.Derivate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), DerivativeMethod.BackwardOnePoint, 0, 1, 100);
        for (int i = 1; i < DsinSum.Length; i++)
            Assert.AreEqual(DsinSum[i], result[i], 1e-12);
    }

    [TestMethod]
    public void Test_Derivative_ForwardOnePoint()
    {
        Dsin1Hz = [6.279051952931340, 6.254271403499090, 6.204808102142040, 6.130857257913020, 6.032710721009260, 5.910755830973060, 5.765473888039470, 5.597438253664260,
            5.407312087728140, 5.195845731347650, 4.963873745621650, 4.712311617999890, 4.442152149272280, 4.154461535437780, 3.850375159915830, 3.531093112706760,
            3.197875454184860, 2.852037242215590, 2.494943342223190, 2.128003040690220, 1.752664483347750, 1.370408960005750, 0.982745058578915, 0.591202711379368,
            0.197327157172844, -0.197327157172844, -0.591202711379379, -0.982745058578915, -1.370408960005760, -1.752664483347750, -2.128003040690230, -2.494943342223190,
            -2.852037242215600, -3.197875454184850, -3.531093112706770, -3.850375159915830, -4.154461535437780, -4.442152149272290, -4.712311617999910, -4.963873745621670,
            -5.195845731347650, -5.407312087728140, -5.597438253664230, -5.765473888039470, -5.910755830973060, -6.032710721009270, -6.130857257913020, -6.204808102142040,
            -6.254271403499090, -6.279051952931330, -6.279051952931340, -6.254271403499090, -6.204808102142040, -6.130857257913020, -6.032710721009270, -5.910755830973050,
            -5.765473888039460, -5.597438253664260, -5.407312087728090, -5.195845731347650, -4.963873745621660, -4.712311617999880, -4.442152149272280, -4.154461535437770,
            -3.850375159915840, -3.531093112706730, -3.197875454184830, -2.852037242215600, -2.494943342223160, -2.128003040690220, -1.752664483347730, -1.370408960005750,
            -0.982745058578892, -0.591202711379368, -0.197327157172822, 0.197327157172866, 0.591202711379379, 0.982745058578948, 1.370408960005760, 1.752664483347760,
            2.128003040690240, 2.494943342223200, 2.852037242215630, 3.197875454184850, 3.531093112706800, 3.850375159915810, 4.154461535437820, 4.442152149272270,
            4.712311617999950, 4.963873745621630, 5.195845731347710, 5.407312087728110, 5.597438253664230, 5.765473888039520, 5.910755830973020, 6.032710721009320,
            6.130857257912980, 6.204808102142090, 6.254271403499050, 6.279051952931390, double.NaN];

        Dsin2Hz = [6.266661678215210, 6.167832680027530, 5.971733275991160, 5.681456070851870, 5.301578909537890, 4.838092681810780, 4.298306842355030, 3.690734136311290,
            3.024956348200230, 2.311473191456700, 1.561536721676750, 0.786973884979148, 0, -0.786973884979141, -1.561536721676750, -2.311473191456710, -3.024956348200220,
            -3.690734136311280, -4.298306842355030, -4.838092681810780, -5.301578909537890, -5.681456070851870, -5.971733275991160, -6.167832680027530, -6.266661678215210,
            -6.266661678215190, -6.167832680027530, -5.971733275991160, -5.681456070851870, -5.301578909537890, -4.838092681810770, -4.298306842355010, -3.690734136311290,
            -3.024956348200220, -2.311473191456700, -1.561536721676750, -0.786973884979130, 0, 0.786973884979158, 1.561536721676770, 2.311473191456720, 3.024956348200240,
            3.690734136311280, 4.298306842355040, 4.838092681810790, 5.301578909537900, 5.681456070851880, 5.971733275991170, 6.167832680027530, 6.266661678215210,
            6.266661678215210, 6.167832680027530, 5.971733275991160, 5.681456070851860, 5.301578909537890, 4.838092681810760, 4.298306842355020, 3.690734136311270,
            3.024956348200180, 2.311473191456680, 1.561536721676740, 0.786973884979125, 0, -0.786973884979169, -1.561536721676800, -2.311473191456710, -3.024956348200220,
            -3.690734136311340, -4.298306842355020, -4.838092681810830, -5.301578909537870, -5.681456070851920, -5.971733275991130, -6.167832680027580, -6.266661678215170,
            -6.266661678215260, -6.167832680027480, -5.971733275991190, -5.681456070851810, -5.301578909537840, -4.838092681810790, -4.298306842354970, -3.690734136311290,
            -3.024956348200170, -2.311473191456680, -1.561536721676710, -0.786973884979108, 0, 0.786973884979191, 1.561536721676790, 2.311473191456760, 3.024956348200240,
            3.690734136311300, 4.298306842355090, 4.838092681810770, 5.301578909537960, 5.681456070851850, 5.971733275991220, 6.167832680027500, 6.266661678215260, double.NaN];

        DsinSum = [12.5457136311465000, 12.4221040835266000, 12.1765413781332000, 11.8123133287649000, 11.3342896305471000, 10.7488485127838000, 10.0637807303945000,
            9.2881723899755500, 8.4322684359283600, 7.5073189228043600, 6.5254104672984000, 5.4992855029790300, 4.4421521492723100, 3.3674876504586200, 2.2888384382390800,
            1.2196199212500600, 0.1729191059846260, -0.8386968940957020, -1.8033635001318400, -2.7100896411205600, -3.5489144261901400, -4.3110471108461300, -4.9889882174122400,
            -5.5766299686481600, -6.0693345210423700, -6.4639888353880400, -6.7590353914069100, -6.9544783345700700, -7.0518650308576200, -7.0542433928856600, -6.9660957225009900,
            -6.7932501845782000, -6.5427713785268800, -6.2228318023850700, -5.8425663041634700, -5.4119118815925700, -4.9414354204169100, -4.4421521492722800, -3.9253377330207500,
            -3.4023370239448900, -2.8843725398909400, -2.3823557395279100, -1.9067041173529500, -1.4671670456844400, -1.0726631491622700, -0.7311318114713690, -0.4494011870611430,
            -0.2330748261508750, -0.0864387234715588, -0.0123902747161245, -0.0123902747161258, -0.0864387234715629, -0.2330748261508830, -0.4494011870611570, -0.7311318114713800,
            -1.0726631491622900, -1.4671670456844500, -1.9067041173529900, -2.3823557395279100, -2.8843725398909700, -3.4023370239449200, -3.9253377330207600, -4.4421521492723000,
            -4.9414354204169400, -5.4119118815926400, -5.8425663041634400, -6.2228318023850500, -6.5427713785269300, -6.7932501845781900, -6.9660957225010500, -7.0542433928856100,
            -7.0518650308576700, -6.9544783345700200, -6.7590353914069500, -6.4639888353880000, -6.0693345210423900, -5.5766299686481100, -4.9889882174122400, -4.3110471108460400,
            -3.5489144261900700, -2.7100896411205600, -1.8033635001317700, -0.8386968940956800, 0.1729191059846920, 1.2196199212501000, 2.2888384382391000, 3.3674876504587300,
            4.4421521492723000, 5.4992855029791300, 6.5254104672984400, 7.5073189228044700, 8.4322684359283500, 9.2881723899755300, 10.0637807303946000, 10.7488485127838000,
            11.3342896305473000, 11.8123133287648000, 12.1765413781333000, 12.4221040835265000, 12.5457136311466000, double.NaN];

        var result = Derivative.Derivate(sin1Hz, DerivativeMethod.ForwardOnePoint, 0, sin1Hz.GetUpperBound(0), 100);
        for (int i = 0; i < Dsin1Hz.Length - 1; i++)
            Assert.AreEqual(Dsin1Hz[i], result[i], 1e-12);

        result = Derivative.Derivate(sin2Hz, DerivativeMethod.ForwardOnePoint, 0, sin1Hz.GetUpperBound(0), 100);
        for (int i = 0; i < Dsin2Hz.Length - 1; i++)
            Assert.AreEqual(Dsin2Hz[i], result[i], 1e-12);

        result = Derivative.Derivate(sinSum, DerivativeMethod.ForwardOnePoint, 0, sinSum.GetUpperBound(0), 100);
        for (int i = 0; i < DsinSum.Length - 1; i++)
            Assert.AreEqual(DsinSum[i], result[i], 1e-12);


        result = Derivative.Derivate((x) => Math.Sin(2 * Math.PI * 1 * x), DerivativeMethod.ForwardOnePoint, 0, 1, 100);
        for (int i = 0; i < Dsin1Hz.Length - 1; i++)
            Assert.AreEqual(Dsin1Hz[i], result[i], 1e-12);

        result = Derivative.Derivate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), DerivativeMethod.ForwardOnePoint, 0, 1, 100);
        for (int i = 0; i < Dsin2Hz.Length - 1; i++)
            Assert.AreEqual(Dsin2Hz[i], result[i], 1e-12);

        result = Derivative.Derivate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), DerivativeMethod.ForwardOnePoint, 0, 1, 100);
        for (int i = 0; i < DsinSum.Length - 1; i++)
            Assert.AreEqual(DsinSum[i], result[i], 1e-12);
    }

    [TestMethod]
    public void Test_Derivative_CenteredThreePoint()
    {
        var result = Derivative.Derivate(sin1Hz, DerivativeMethod.CenteredThreePoint, 0, sin1Hz.GetUpperBound(0), 100);
        for (int i = 1; i < Dsin1Hz.Length - 1; i++)
            Assert.AreEqual(Dsin1Hz[i], result[i], 1e-1);

        result = Derivative.Derivate(sin2Hz, DerivativeMethod.CenteredThreePoint, 0, sin1Hz.GetUpperBound(0), 100);
        for (int i = 1; i < Dsin2Hz.Length - 1; i++)
            Assert.AreEqual(Dsin2Hz[i], result[i], 1e-1);

        result = Derivative.Derivate(sinSum, DerivativeMethod.CenteredThreePoint, 0, sinSum.GetUpperBound(0), 100);
        for (int i = 1; i < DsinSum.Length - 1; i++)
            Assert.AreEqual(DsinSum[i], result[i], 1e-1);


        result = Derivative.Derivate((x) => Math.Sin(2 * Math.PI * 1 * x), DerivativeMethod.CenteredThreePoint, 0, 1, 100);
        for (int i = 1; i < Dsin1Hz.Length - 1; i++)
            Assert.AreEqual(Dsin1Hz[i], result[i], 1e-1);

        result = Derivative.Derivate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), DerivativeMethod.CenteredThreePoint, 0, 1, 100);
        for (int i = 1; i < Dsin2Hz.Length - 1; i++)
            Assert.AreEqual(Dsin2Hz[i], result[i], 1e-1);

        result = Derivative.Derivate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), DerivativeMethod.CenteredThreePoint, 0, 1, 100);
        for (int i = 1; i < DsinSum.Length - 1; i++)
            Assert.AreEqual(DsinSum[i], result[i], 1e-1);
    }

    [TestMethod]
    public void Test_Derivative_CenteredFivePoint()
    {
        var result = Derivative.Derivate(sin1Hz, DerivativeMethod.CenteredFivePoint, 0, sin1Hz.GetUpperBound(0), 100);
        for (int i = 2; i < Dsin1Hz.Length - 2; i++)
            Assert.AreEqual(Dsin1Hz[i], result[i], 1e-4);

        result = Derivative.Derivate(sin2Hz, DerivativeMethod.CenteredFivePoint, 0, sin1Hz.GetUpperBound(0), 100);
        for (int i = 2; i < Dsin2Hz.Length - 2; i++)
            Assert.AreEqual(Dsin2Hz[i], result[i], 1e-4);

        result = Derivative.Derivate(sinSum, DerivativeMethod.CenteredFivePoint, 0, sinSum.GetUpperBound(0), 100);
        for (int i = 2; i < DsinSum.Length - 2; i++)
            Assert.AreEqual(DsinSum[i], result[i], 1e-4);


        result = Derivative.Derivate((x) => Math.Sin(2 * Math.PI * 1 * x), DerivativeMethod.CenteredFivePoint, 0, 1, 100);
        for (int i = 2; i < Dsin1Hz.Length - 2; i++)
            Assert.AreEqual(Dsin1Hz[i], result[i], 1e-4);

        result = Derivative.Derivate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), DerivativeMethod.CenteredFivePoint, 0, 1, 100);
        for (int i = 2; i < Dsin2Hz.Length - 2; i++)
            Assert.AreEqual(Dsin2Hz[i], result[i], 1e-4);

        result = Derivative.Derivate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), DerivativeMethod.CenteredFivePoint, 0, 1, 100);
        for (int i = 2; i < DsinSum.Length - 2; i++)
            Assert.AreEqual(DsinSum[i], result[i], 1e-4);
    }

    [TestMethod]
    public void Test_Derivative_CenteredSevenPoint()
    {
        var result = Derivative.Derivate(sin1Hz, DerivativeMethod.CenteredSevenPoint, 0, sin1Hz.GetUpperBound(0), 100);
        for (int i = 3; i < Dsin1Hz.Length - 3; i++)
            Assert.AreEqual(Dsin1Hz[i], result[i], 1e-6);

        result = Derivative.Derivate(sin2Hz, DerivativeMethod.CenteredSevenPoint, 0, sin1Hz.GetUpperBound(0), 100);
        for (int i = 3; i < Dsin2Hz.Length - 3; i++)
            Assert.AreEqual(Dsin2Hz[i], result[i], 1e-6);

        result = Derivative.Derivate(sinSum, DerivativeMethod.CenteredSevenPoint, 0, sinSum.GetUpperBound(0), 100);
        for (int i = 3; i < DsinSum.Length - 3; i++)
            Assert.AreEqual(DsinSum[i], result[i], 1e-6);


        result = Derivative.Derivate((x) => Math.Sin(2 * Math.PI * 1 * x), DerivativeMethod.CenteredSevenPoint, 0, 1, 100);
        for (int i = 3; i < Dsin1Hz.Length - 3; i++)
            Assert.AreEqual(Dsin1Hz[i], result[i], 1e-6);

        result = Derivative.Derivate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), DerivativeMethod.CenteredSevenPoint, 0, 1, 100);
        for (int i = 3; i < Dsin2Hz.Length - 3; i++)
            Assert.AreEqual(Dsin2Hz[i], result[i], 1e-6);

        result = Derivative.Derivate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), DerivativeMethod.CenteredSevenPoint, 0, 1, 100);
        for (int i = 3; i < DsinSum.Length - 3; i++)
            Assert.AreEqual(DsinSum[i], result[i], 1e-6);
    }

    [TestMethod]
    public void Test_Derivative_CenteredNinePoint()
    {
        var result = Derivative.Derivate(sin1Hz, DerivativeMethod.CenteredNinePoint, 0, sin1Hz.GetUpperBound(0), 100);
        for (int i = 4; i < Dsin1Hz.Length - 4; i++)
            Assert.AreEqual(Dsin1Hz[i], result[i], 1e-9);

        result = Derivative.Derivate(sin2Hz, DerivativeMethod.CenteredNinePoint, 0, sin1Hz.GetUpperBound(0), 100);
        for (int i = 4; i < Dsin2Hz.Length - 4; i++)
            Assert.AreEqual(Dsin2Hz[i], result[i], 1e-9);

        result = Derivative.Derivate(sinSum, DerivativeMethod.CenteredNinePoint, 0, sinSum.GetUpperBound(0), 100);
        for (int i = 4; i < DsinSum.Length - 4; i++)
            Assert.AreEqual(DsinSum[i], result[i], 1e-8);


        result = Derivative.Derivate((x) => Math.Sin(2 * Math.PI * 1 * x), DerivativeMethod.CenteredNinePoint, 0, 1, 100);
        for (int i = 4; i < Dsin1Hz.Length - 4; i++)
            Assert.AreEqual(Dsin1Hz[i], result[i], 1e-9);

        result = Derivative.Derivate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), DerivativeMethod.CenteredNinePoint, 0, 1, 100);
        for (int i = 4; i < Dsin2Hz.Length - 4; i++)
            Assert.AreEqual(Dsin2Hz[i], result[i], 1e-9);

        result = Derivative.Derivate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), DerivativeMethod.CenteredNinePoint, 0, 1, 100);
        for (int i = 4; i < DsinSum.Length - 4; i++)
            Assert.AreEqual(DsinSum[i], result[i], 1e-8);
    }

    [TestMethod]
    public void Test_Derivative_SGLinearThreePoint()
    {
        var result = Derivative.Derivate(sin1Hz, DerivativeMethod.SGLinearThreePoint, 0, sin1Hz.GetUpperBound(0), 100);
        for (int i = 1; i < Dsin1Hz.Length - 1; i++)
            Assert.AreEqual(Dsin1Hz[i], result[i], 1e-1);

        result = Derivative.Derivate(sin2Hz, DerivativeMethod.SGLinearThreePoint, 0, sin1Hz.GetUpperBound(0), 100);
        for (int i = 1; i < Dsin2Hz.Length - 1; i++)
            Assert.AreEqual(Dsin2Hz[i], result[i], 1e-1);

        result = Derivative.Derivate(sinSum, DerivativeMethod.SGLinearThreePoint, 0, sinSum.GetUpperBound(0), 100);
        for (int i = 1; i < DsinSum.Length - 1; i++)
            Assert.AreEqual(DsinSum[i], result[i], 1e-1);


        result = Derivative.Derivate((x) => Math.Sin(2 * Math.PI * 1 * x), DerivativeMethod.SGLinearThreePoint, 0, 1, 100);
        for (int i = 1; i < Dsin1Hz.Length - 1; i++)
            Assert.AreEqual(Dsin1Hz[i], result[i], 1e-1);

        result = Derivative.Derivate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), DerivativeMethod.SGLinearThreePoint, 0, 1, 100);
        for (int i = 1; i < Dsin2Hz.Length - 1; i++)
            Assert.AreEqual(Dsin2Hz[i], result[i], 1e-1);

        result = Derivative.Derivate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), DerivativeMethod.SGLinearThreePoint, 0, 1, 100);
        for (int i = 1; i < DsinSum.Length - 1; i++)
            Assert.AreEqual(DsinSum[i], result[i], 1e-1);
    }

    [TestMethod]
    public void Test_Derivative_SGLinearFivePoint()
    {
        var result = Derivative.Derivate(sin1Hz, DerivativeMethod.SGLinearFivePoint, 0, sin1Hz.GetUpperBound(0), 100);
        for (int i = 2; i < Dsin1Hz.Length - 2; i++)
            Assert.AreEqual(Dsin1Hz[i], result[i], 1e-1);

        result = Derivative.Derivate(sin2Hz, DerivativeMethod.SGLinearFivePoint, 0, sin1Hz.GetUpperBound(0), 100);
        for (int i = 2; i < Dsin2Hz.Length - 2; i++)
            Assert.AreEqual(Dsin2Hz[i], result[i], 1e-1);

        result = Derivative.Derivate(sinSum, DerivativeMethod.SGLinearFivePoint, 0, sinSum.GetUpperBound(0), 100);
        for (int i = 2; i < DsinSum.Length - 2; i++)
            Assert.AreEqual(DsinSum[i], result[i], 1e-1);


        result = Derivative.Derivate((x) => Math.Sin(2 * Math.PI * 1 * x), DerivativeMethod.SGLinearFivePoint, 0, 1, 100);
        for (int i = 2; i < Dsin1Hz.Length - 2; i++)
            Assert.AreEqual(Dsin1Hz[i], result[i], 1e-1);

        result = Derivative.Derivate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), DerivativeMethod.SGLinearFivePoint, 0, 1, 100);
        for (int i = 2; i < Dsin2Hz.Length - 2; i++)
            Assert.AreEqual(Dsin2Hz[i], result[i], 1e-1);

        result = Derivative.Derivate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), DerivativeMethod.SGLinearFivePoint, 0, 1, 100);
        for (int i = 2; i < DsinSum.Length - 2; i++)
            Assert.AreEqual(DsinSum[i], result[i], 1e-1);
    }

    [TestMethod]
    public void Test_Derivative_SGLinearSevenPoint()
    {

    }

    [TestMethod]
    public void Test_Derivative_SGLinearNinePoint()
    {

    }

    [TestMethod]
    public void Test_Derivative_SGCubicFivePoint()
    {

    }

    [TestMethod]
    public void Test_Derivative_SGCubicSevenPoint()
    {

    }

    [TestMethod]
    public void Test_Derivative_SGCubicNinePoint()
    {

    }
}
