import { useScrollReveal } from '../../hooks/useScrollReveal'

export default function ScrollReveal({ children, className = '', delay = 0 }) {
  const { ref, visible } = useScrollReveal()

  return (
    <div
      ref={ref}
      className={`transition-all duration-700 ease-out ${
        visible ? 'translate-y-0 opacity-100' : 'translate-y-8 opacity-0'
      } ${className}`}
      style={{ transitionDelay: `${delay}ms` }}
    >
      {children}
    </div>
  )
}
