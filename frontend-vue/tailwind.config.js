/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
      "./index.html",
      "./src/**/*.{vue,js,ts}"
  ],
  theme: {
    extend: {
        keyframes: {
            fadeIn: {
                '0%': { opacity: 0 },
                '100%': { opacity: 1 }
            },
            wiggle: {
                '0%': { transform: 'rotate(0deg)' },
                '10%': { transform: 'rotate(0deg)' },
                '15%': { transform: 'rotate(3deg)' },
                '25%': { transform: 'rotate(-3deg)' },
                '30%': { transform: 'rotate(0deg)' },
            }
        },
        animation: {
            fadeIn: 'fadeIn .2s ease-in',
            wiggle: 'wiggle 1s ease-in-out'
        }
    },
    colors: {
        background: '#191B20',
        backgroundDark: '#262A31',
        foreground: '#292B32',
        primary: '#A003F8',
        primaryDark: '#9B02F0',
        secondary: '#3342CB',
        secondaryActive: '#263AA6',
        secondaryHover: '#2D3BC1',
        text: '#E6E6E6',
        textDark: '#D0D0D0',
        textGrey: '#BBBBBB',
        line: '#525459',
        lineDark: '#232323',
        edit: '#3498DB',
        delete: '#E74C3C',
        save: '#2ECC71'
    }
  },
  plugins: [],
}
