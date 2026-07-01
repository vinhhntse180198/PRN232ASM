/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,jsx,ts,tsx}'],
  theme: {
    extend: {
      colors: {
        base: 'rgb(var(--c-base) / <alpha-value>)',
        surface: 'rgb(var(--c-surface) / <alpha-value>)',
        elevated: 'rgb(var(--c-elevated) / <alpha-value>)',
        accent: {
          primary: 'rgb(var(--c-accent-primary) / <alpha-value>)',
          glow: 'rgb(var(--c-accent-glow) / <alpha-value>)',
          green: 'rgb(var(--c-accent-green) / <alpha-value>)',
          amber: 'rgb(var(--c-accent-amber) / <alpha-value>)',
        },
        primary: 'rgb(var(--c-primary) / <alpha-value>)',
        muted: 'rgb(var(--c-muted) / <alpha-value>)',
        border: 'rgb(var(--c-border) / <alpha-value>)',
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
        display: ['"DM Sans"', 'Inter', 'sans-serif'],
        serif: ['"Source Serif 4"', 'Georgia', 'serif'],
        mono: ['"JetBrains Mono"', 'monospace'],
      },
      boxShadow: {
        glow: '0 0 60px -12px rgba(99, 102, 241, 0.45)',
        'glow-sm': '0 0 30px -8px rgba(99, 102, 241, 0.35)',
        card: '0 4px 24px rgba(0, 0, 0, 0.4)',
      },
      animation: {
        float: 'float 3s ease-in-out infinite',
      },
      keyframes: {
        float: {
          '0%, 100%': { transform: 'translateY(0)' },
          '50%': { transform: 'translateY(-8px)' },
        },
      },
      backgroundImage: {
        'hero-gradient': 'linear-gradient(135deg, #6366F1 0%, #A78BFA 100%)',
        'cta-gradient': 'linear-gradient(135deg, #312E81 0%, #1E1B4B 50%, #0A0B1E 100%)',
        'halo': 'radial-gradient(ellipse at center, rgba(99,102,241,0.15) 0%, transparent 70%)',
      },
    },
  },
  plugins: [],
}
