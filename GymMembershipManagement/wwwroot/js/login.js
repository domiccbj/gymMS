/* Particles.js konfiguracija za krugove */
particlesJS("particles-js", {
    particles: {
        number: {
            value: 120, // Broj krugova
            density: {
                enable: true,
                value_area: 800, // Prostor za širenje krugova
            },
        },
        color: {
            value: "#ffffff", // Boja krugova
        },
        shape: {
            type: "circle", // Oblik: krug
            stroke: {
                width: 0,
                color: "#000000",
            },
        },
        opacity: {
            value: 0.8, // Prozirnost krugova
            random: true, // Nasumična prozirnost
            anim: {
                enable: true,
                speed: 2,
                opacity_min: 0.1,
                sync: false,
            },
        },
        size: {
            value: 2, // Veličina krugova
            random: true, // Nasumična veličina
            anim: {
                enable: false,
                speed: 40,
                size_min: 0.1,
                sync: false,
            },
        },
        line_linked: {
            enable: false, // Ukloni linije
        },
        move: {
            enable: true,
            speed: 2, // Brzina kretanja krugova
            direction: "none",
            random: true,
            straight: false,
            out_mode: "out",
            bounce: false,
            attract: {
                enable: false,
                rotateX: 600,
                rotateY: 1200,
            },
        },
    },
    interactivity: {
        detect_on: "canvas",
        events: {
            onhover: {
                enable: true,
                mode: "grab", // Efekt grabanja prilikom prelaska miša
            },
            onclick: {
                enable: true,
                mode: "repulse", // Efekt udaljavanja prilikom klika
            },
            resize: true,
        },
        modes: {
            grab: {
                distance: 200, // Udaljenost grabanja
                line_linked: {
                    opacity: 1,
                },
            },
            bubble: {
                distance: 400,
                size: 40,
                duration: 2,
                opacity: 8,
                speed: 2,
            },
            repulse: {
                distance: 200,
                duration: 0.4,
            },
            push: {
                particles_nb: 4,
            },
            remove: {
                particles_nb: 2,
            },
        },
    },
    retina_detect: true,
});
